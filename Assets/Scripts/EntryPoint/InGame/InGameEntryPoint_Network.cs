using Domain.Action;
using Domain.Component;
using Domain.Game;
using Domain.Player;
using Domain.Stage;
using Infrastracture.Network;
using Infrastracture.Repository;
using Infrastructure.Network;
using Infrastructure.Repository;
using Presenter.Network;
using Presenter.Player;
using UnityEngine;
using UseCase.Game;
using UseCase.GameSystem;
using UseCase.Player;
using UseCase.Player.Network;
using UseCase.Stage;
using View.Player;
using View.Stage;
using View.UI;

public class InGameEntryPoint_Network : MonoBehaviour
{
    //Player
    [SerializeField]
    PlayerView view;
    [SerializeField]
    RaycastController raycast;
    [SerializeField]
    InputController input;
    [SerializeField]
    PlayerCarryView carryView;
    [SerializeField]
    PlayerActionExecuter executer;
    [SerializeField]
    InteractView interact;

    //UI
    [SerializeField]
    ObjectInfoView infoView;
    [SerializeField]
    ActionOverlayView actionOverlayView;
    [SerializeField]
    ResultView resultView;
    [SerializeField]
    DocumentView documentView;

    PlayerSystemUseCase usecase;
    IObjectRepository repository;
    StageEntity stage;

    bool IsActive = false;

    public void Entry()
    {
        repository = FindFirstObjectByType<ObjectRepositoryHolder>().repository;
        var session = FindFirstObjectByType<SessionHolder>().room;
        var stageRepositoryHolder = FindFirstObjectByType<StageRepositoryHolder>();
        var stageRepository = stageRepositoryHolder.repository;
        stage = stageRepository.GetCurrentStageEntity();

        var gameState = new GameStateManager();

        var model = new PlayerEntity(view.Position, view.Rotation);

        var move = new PlayerMoveController(view, model);

        var socket = FindFirstObjectByType<NativeWebSocketService>();
        var sender = new PacketSender(socket);
        var presenter = new InspectPresenter(input, infoView);
        var inspect = new RemoteInspectUseCase(presenter, new InspectService(), repository, sender, session);

        var actionService = new ActionService();
        var actionPresenter = new ActionPresenter(actionOverlayView, input);
        var action = new RemoteActionUseCase(model, actionPresenter, executer, repository, stage, actionService, sender, session);
        
        // Presenter‚ÍView‚Ì‚Ý‚ð’m‚é
        var carryPresenter = new PlayerCarryPresenter(carryView);

        // UseCase‚ÍPresenter‚ð—˜—p‚·‚é
        var carry = new PlayerCarryUseCase(model, carryPresenter, repository);

        var document = new DocumentUseCase(documentView, new DocumentEntity());

        usecase = new PlayerSystemUseCase(move, inspect, model, input, gameState, raycast, carry, action, new InteractUseCase(repository, interact));


        var gameSystem = new GameSystemUseCase(usecase, new StageSystemUseCase(stage, resultView), gameState, document, input);

        gameSystem.StartGame();

        IsActive = true;
    }
    private void Start()
    {
        if (!IsActive)
            gameObject.SetActive(false);
    }
    void Update()
    {
        usecase.Update();
    }

    void LateUpdate()
    {
        usecase.LateUpdate();
    }
}

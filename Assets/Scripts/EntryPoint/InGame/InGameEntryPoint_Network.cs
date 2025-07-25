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
using Presenter.Sound;
using Presenter.Vote;
using UnityEngine;
using UnityEngine.Video;
using UseCase.Game;
using UseCase.GameSystem;
using UseCase.Network;
using UseCase.Player;
using UseCase.Player.Network;
using UseCase.Stage;
using View.Network;
using View.Player;
using View.Sound;
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

    [SerializeField]
    RemotePlayerViewFactory remotePlayerFactory;

    //UI
    [SerializeField]
    ObjectInfoView infoView;
    [SerializeField]
    ActionOverlayView actionOverlayView;
    [SerializeField]
    ResultView resultView;
    [SerializeField]
    DocumentView documentView;
    [SerializeField]
    ActionHintUI hintUI;
    [SerializeField]
    VoteView voteView;

    //Sound
    [SerializeField]
    SoundView sound;

    PlayerSystemUseCase usecase;
    IObjectRepository repository;
    StageEntity stage;



    bool IsActive = false;

    public void Entry()
    {
        repository = FindFirstObjectByType<ObjectRepositoryHolder>().repository;
        var session = FindFirstObjectByType<SessionHolder>().room;
        var receiver = FindFirstObjectByType<ReceiverHolder>().receiver;
        var stageRepository = FindFirstObjectByType<StageRepositoryHolder>().repository;
        stage = stageRepository.GetCurrentStageEntity();

        var soundPresenter = new SoundPresenter(sound);

        var gameState = new GameStateManager();

        var model = new PlayerEntity(view.Position, view.Rotation);

        var socket = FindFirstObjectByType<NativeWebSocketService>();
        var sender = new PacketSender(socket);

        var move = new RemotePlayerMoveController(model, view, sender, session);

        var presenter = new InspectPresenter(input, infoView, soundPresenter);
        
        var inspect = new RemoteInspectUseCase(presenter, new InspectService(), repository, sender, session);

        var actionService = new ActionService();
        var actionPresenter = new ActionPresenter(actionOverlayView, input);
        var action = new RemoteActionUseCase(model, actionPresenter, executer, repository, stage, actionService, sender, receiver, session, stageRepository);

        var carryPresenter = new PlayerCarryPresenter(carryView);

        var carry = new PlayerCarryUseCase(model, carryPresenter, repository);

        var document = new DocumentUseCase(documentView, new DocumentEntity());

        var hintPresenter = new ActionHintPresenter(hintUI);
        usecase = new PlayerSystemUseCase(move, inspect, model, input, gameState, raycast, carry, action, new InteractUseCase(repository, interact), hintPresenter);

        var remotePlayerRepository = new RemotePlayerRepository();
        var syncUseCase = new RemotePlayerSyncUseCase(remotePlayerRepository, remotePlayerFactory, session, receiver);

        
        var voteUseCase = new VoteUseCase(sender, receiver, session);
        var gameSystem = new RemoteGameSystemUseCase(usecase, new StageSystemUseCase(stage, resultView), gameState, document, input, voteUseCase, session, sender, receiver);
        var votePresenter = new VotePresenter(voteUseCase, gameSystem, voteView);
        voteView.Inject(votePresenter);

        gameSystem.StartGame();

        IsActive = true;
    }
    private void Start()
    {
        if (!IsActive)
            gameObject.SetActive(false);
    }
    async void Update()
    {
        await usecase.Update();
    }

    void LateUpdate()
    {
        usecase.LateUpdate();
    }
}

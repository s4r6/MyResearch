using Domain.Action;
using Domain.Game;
using Domain.Player;
using Domain.Stage;
using Infrastructure.Factory;
using Infrastructure.Game;
using Infrastructure.Network;
using Infrastructure.Repository;
using Presenter.Player;
using Presenter.Sound;
using UnityEngine;
using UseCase.Game;
using UseCase.GameSystem;
using UseCase.Player;
using UseCase.Stage;
using View.Player;
using View.Sound;
using View.Stage;
using View.UI;

public class InGameEntryPoint_Local : MonoBehaviour
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
    [SerializeField]
    ActionHintUI hintUI;
    [SerializeField]
    GameObject Reticle;

    //Sound
    [SerializeField]
    SoundView sound;


    PlayerSystemUseCase usecase;
    ObjectRepository repository;
    StageEntity stage;

    bool IsActive = false;

    GameSystemUseCase gameSystem;

    public void Entry()
    {
        var gameMode = FindFirstObjectByType<GameModeHolder>();


        var entityFactory = new EntityFactory("Master/Stage1/");
        repository = new ObjectRepository(entityFactory, "Master/Stage1/");
        var stageRepository = new StageRepository(repository);
        stage = stageRepository.CreateStage(1, repository);

        var soundPresenter = new SoundPresenter(sound);

        var gameState = new GameStateManager();

        var model = new PlayerEntity(view.Position, view.Rotation);

        var move = new PlayerMoveController(view, model);


        var presenter = new InspectPresenter(input, infoView, soundPresenter);
        var inspect = new PlayerInspectUseCase(model, presenter, new InspectService(), repository);

        var actionService = new ActionService();
        var actionPresenter = new ActionPresenter(actionOverlayView, input);
        var action = new PlayerActionUseCase(model, actionPresenter, executer, repository, stage, actionService);
        // Presenter‚ÍView‚Ì‚Ý‚ð’m‚é
        var carryPresenter = new PlayerCarryPresenter(carryView);

        // UseCase‚ÍPresenter‚ð—˜—p‚·‚é
        var carry = new PlayerCarryUseCase(model, carryPresenter, repository);

        var document = new DocumentUseCase(documentView, new DocumentEntity());

        var hintPresenter = new ActionHintPresenter(hintUI);
        usecase = new PlayerSystemUseCase(move, inspect, model, input, gameState, raycast, carry, action, new InteractUseCase(repository, interact), hintPresenter, Reticle);

        var gameEntity = new GameEntity(stage, gameState);
        gameSystem = new GameSystemUseCase(resultView, gameEntity, input);

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
        usecase?.Update();
    }

    void LateUpdate()
    {
        usecase?.LateUpdate();
    }

    private void OnDestroy()
    {
        usecase?.Dispose();  //PlayerSystem‚ÌŒã•Ð•t‚¯
        gameSystem?.Dispose();
    }
}

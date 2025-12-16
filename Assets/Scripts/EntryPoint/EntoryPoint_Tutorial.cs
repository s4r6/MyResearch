using Cysharp.Threading.Tasks;
using Domain.Action;
using Domain.Game;
using Domain.Player;
using Domain.Stage;
using Domain.Tutorial;
using Infrastructure.Factory;
using Infrastructure.Game;
using Infrastructure.Repository;
using Presenter.Player;
using Presenter.Sound;
using Presenter.Tutorial;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UseCase.Game;
using UseCase.GameSystem;
using UseCase.Player;
using UseCase.Stage;
using View.Player;
using View.Sound;
using View.Stage;
using View.Tutorial;
using View.UI;

public class EntoryPoint_Tutorial : MonoBehaviour
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

    [SerializeField]
    TutorialWindowView tutorialWindow;
    [SerializeField]
    TutorialHighlightTarget target;

    GameSystemUseCase gameSystem;

    TutorialFlowController tutorialFlow;
    private async UniTask Awake()
    {

        Entry();
        gameSystem.StartGame().Forget();

        _cts = new CancellationTokenSource();

        // ここでは「ゲーム開始＝チュートリアル開始」として即実行
        await tutorialFlow.RunAllAsync(_cts.Token);

        // 全フェーズ終了後、ここで何かあれば書く（チュートリアル終了フラグなど）
        Debug.Log("Tutorial finished.");
    }


    CancellationTokenSource _cts;
    public async void Entry()
    {
        var tutorialPhase = new TutorialPhaseState();
        var gameMode = FindFirstObjectByType<GameModeHolder>();

        var entityFactory = new EntityFactory();
        repository = new ObjectRepository(entityFactory);
        var stageRepository = new StageRepository(repository);
        stage = stageRepository.CreateStage(1, repository);

        var soundPresenter = new SoundPresenter(sound);

        var gameState = new GameStateManager();

        var model = new PlayerEntity(view.Position, view.Rotation);

        var move = new PlayerMoveController(view, model);


        var presenter = new InspectPresenter(input, infoView, soundPresenter);
        var inspect = new PlayerInspectUseCase(model, presenter, new InspectService(), repository, new TutorialRestriction(tutorialPhase));

        var actionService = new ActionService();
        var actionPresenter = new ActionPresenter(actionOverlayView, input);
        var action = new PlayerActionUseCase(model, actionPresenter, executer, repository, stage, actionService);
        // PresenterはViewのみを知る
        var carryPresenter = new PlayerCarryPresenter(carryView);

        // UseCaseはPresenterを利用する
        var carry = new PlayerCarryUseCase(model, carryPresenter, repository);

        var document = new DocumentUseCase(documentView, new DocumentEntity());

        var hintPresenter = new ActionHintPresenter(hintUI);
        usecase = new PlayerSystemUseCase(move, inspect, model, input, gameState, raycast, carry, action, new InteractUseCase(repository, interact), hintPresenter, Reticle);
        
        gameSystem = new GameSystemUseCase(resultView, new GameEntity(stage, gameState), input);

        var tutorialInput = FindFirstObjectByType<TutorialInput>();
        var actionMapController = FindFirstObjectByType<PlayerInputActionMapController>();

        var eventAdapter = new TutorialGameEventsAdapter(documentView, inspect, action, carry, gameSystem);
        var tutorialhighlightView = FindFirstObjectByType<TutorialHighlightView>();
        tutorialFlow = new TutorialFlowController(tutorialPhase, tutorialWindow, tutorialInput, actionMapController, eventAdapter, tutorialhighlightView, inspect, "一般業務用PC01", "文字の書かれた付箋", "文書庫２");

        
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
        usecase.Dispose();
        gameSystem.Dispose();

        _cts?.Cancel();
        _cts?.Dispose();
    }
}

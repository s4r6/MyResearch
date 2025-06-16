using Domain.Player;
using UnityEngine;
using UseCase.Player;
using View.Player;
using View.UI;
using Domain.Game;
using Presenter.Player;

using Domain.Action;
using Infrastructure.Repository;
using UseCase.GameSystem;
using UseCase.Stage;
using Infrastructure.Factory;
using Domain.Stage;
using View.Stage;
using UseCase.Game;
using Infrastructure.Game;
using Infrastracture.Network;
using Infrastructure.Network;

namespace EntryPoint
{
    public class EntryPoint : MonoBehaviour
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
        void Awake()
        {
            var gameMode = FindFirstObjectByType<GameModeHolder>();
            

            if(gameMode?.CurrentMode == GameMode.Solo)
            {
                var entityFactory = new EntityFactory();
                repository = new ObjectRepository(entityFactory);
                var stageRepository = new StageRepository(repository);
                var maxRiskAmount = stageRepository.GetRiskAmountByStageNumber(1);
                var maxActionPointAmount = stageRepository.GetActionPointAmountByStageNumber(1);
                stage = new StageEntity(maxRiskAmount, maxActionPointAmount);
            }
            else if(gameMode?.CurrentMode == GameMode.Multi)
            {
                repository = FindFirstObjectByType<ObjectRepositoryHolder>().repository;
                stage = new StageEntity(100, 100);
            }
            else
            {
                var entityFactory = new EntityFactory();
                repository = new ObjectRepository(entityFactory);
                var stageRepository = new StageRepository(repository);
                var maxRiskAmount = stageRepository.GetRiskAmountByStageNumber(1);
                var maxActionPointAmount = stageRepository.GetActionPointAmountByStageNumber(1);
                stage = new StageEntity(maxRiskAmount, maxActionPointAmount);
            }

                var gameState = new GameStateManager();

            var model = new PlayerEntity(view.Position, view.Rotation);

            var move = new PlayerMoveController(view, model);
            var inspect = new PlayerInspectUseCase(model, infoView, repository);

            var action = new PlayerActionUseCase(model, actionOverlayView,executer, repository, stage);
            // PresenterはViewのみを知る
            var carryPresenter = new PlayerCarryPresenter(carryView);
            
            // UseCaseはPresenterを利用する
            var carry = new PlayerCarryUseCase(model, carryPresenter, repository);

            var document = new DocumentUseCase(documentView, new DocumentEntity());

            usecase = new PlayerSystemUseCase(move, inspect, model, input, gameState, raycast, carry, action, new InteractUseCase(repository, interact));

            
            var gameSystem = new GameSystemUseCase(usecase, new StageSystemUseCase(stage, resultView), gameState, document, input);
        
            gameSystem.StartGame();
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
}


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

        //UI
        [SerializeField]
        ObjectInfoView infoView;
        [SerializeField]
        ActionOverlayView actionOverlayView;

        PlayerSystemUseCase usecase;
        void Awake()
        {
            var entityFactory = new EntityFactory();

            var repository = new ObjectRepository(entityFactory);

            var gameState = new GameStateManager();

            var model = new PlayerEntity(view.Position, view.Rotation);

            var move = new PlayerMoveController(view, model);
            var inspect = new PlayerInspectUseCase(model, infoView, repository);

            var executor = new ActionExecuter();
            var action = new PlayerActionUseCase(model, actionOverlayView,executor ,executer, repository);
            // PresenterはViewのみを知る
            var carryPresenter = new PlayerCarryPresenter(carryView);
            
            // UseCaseはPresenterを利用する
            var carry = new PlayerCarryUseCase(model, carryPresenter, repository);
            
            usecase = new PlayerSystemUseCase(move, inspect, model, input, gameState, raycast, carry, action);

            //var gameSystem = new GameSystemUseCase(usecase, new StageSystemUseCase(stage));
        
            //gameSystem.StartGame();
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


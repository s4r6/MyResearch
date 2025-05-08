using Domain.Player;
using Infrastructure.Stage.Object;
using UnityEngine;
using Infrastructure.Master;
using UseCase.Player;
using View.Player;
using View.UI;
using Domain.Game;
using Presenter.Player;
using Infrastructure.Action;
using Domain.Action;

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
            var gameState = new GameStateManager();

            var model = new PlayerEntity(view.Position, view.Rotation);
            var provider = new LocalMasterDataProvider();
            var objectRepository = new InspectableObjectRepository(provider);
            var actionRepository = new ActionRepository(provider);

            var move = new PlayerMoveController(view, model);
            var inspect = new PlayerInspectUseCase(model, objectRepository, actionRepository, infoView);
            var action = new PlayerActionUseCase(model, objectRepository, actionRepository, actionOverlayView, new ActionRuleService(), executer);
            // PresenterはViewのみを知る
            var carryPresenter = new PlayerCarryPresenter(carryView);
            
            // UseCaseはPresenterを利用する
            var carry = new PlayerCarryUseCase(model, objectRepository, carryPresenter);
            
            usecase = new PlayerSystemUseCase(move, inspect, model, input, gameState, raycast, carry, action);
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


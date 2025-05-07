using Domain.Player;
using Infrastructure.Stage.Object;
using UnityEngine;
using Infrastructure.Master;
using UseCase.Player;
using View.Player;
using View.UI;
using Domain.Game;

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

        //UI
        [SerializeField]
        ObjectInfoView infoView;

        PlayerSystemUseCase usecase;
        void Awake()
        {
            var gameState = new GameStateManager();

            var model = new PlayerEntity(view.Position, view.Rotation);
            var objectRepository = new InspectableObjectRepository(new LocalMasterDataProvider());

            var move = new PlayerMoveController(view, model);
            var inspect = new PlayerInspectUseCase(model, raycast, objectRepository, infoView);

            usecase = new PlayerSystemUseCase(move, inspect, model, input, gameState);
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


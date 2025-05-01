using Domain.Player;
using Infrastructure.Stage.Object;
using UnityEngine;
using Infrastructure.Master;
using UseCase.Player;
using View.Player;

namespace EntryPoint
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField]
        PlayerView view;
        [SerializeField]
        RaycastController raycast;
        [SerializeField]
        InputController input;

        PlayerSystemUseCase usecase;
        void Awake()
        {
            var model = new PlayerEntity(view.Position, view.Rotation);
            var objectRepository = new InspectableObjectRepository(new LocalMasterDataProvider());

            var move = new PlayerMoveController(view, model);
            var inspect = new PlayerInspectUseCase(model, raycast, objectRepository);

            usecase = new PlayerSystemUseCase(move, inspect, model, input);
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


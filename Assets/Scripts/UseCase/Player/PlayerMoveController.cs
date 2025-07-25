using Cysharp.Threading.Tasks;
using Domain.Player;
using UnityEngine;
using UseCase.Network;
using View.Player;

namespace UseCase.Player
{
    public class PlayerMoveController : IMoveController
    {
        PlayerEntity model;
        PlayerView view;

        public PlayerMoveController(PlayerView view, PlayerEntity model)
        {
            this.view = view;
            this.model = model;
        }

        void TryMove(Vector2 inputDirection)
        {
            var position = view.Move(inputDirection.normalized, model.speed);
            model.SetPosition(position);
        }

        public UniTask OnMoveInput(Vector2 direction)
        {
            TryMove(direction);
            return UniTask.CompletedTask;
        }

        void TryRotate(float yaw, float pitch)
        {
            var rotation = view.Rotate(yaw, pitch);
            model.SetRotation(rotation);
        }

        public UniTask OnLookInput(Vector2 delta)
        {
            float yaw = delta.x * model.lookSensitivity.x;
            float pitch = delta.y * model.lookSensitivity.y;
            
            TryRotate(yaw, pitch);
            return UniTask.CompletedTask;
        }
    }
}

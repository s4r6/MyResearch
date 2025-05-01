using Domain.Player;
using UnityEngine;
using View.Player;

namespace UseCase.Player
{
    public class PlayerMoveController
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

        public void OnMoveInput(Vector2 direction)
        {
            if (!model.isMovable)
                return;

            TryMove(direction);
        }

        void TryRotate(float yaw, float pitch)
        {
            var rotation = view.Rotate(yaw, pitch);
            model.SetRotation(rotation);
        }

        public void OnLookInput(Vector2 delta)
        {
            if (!model.isLookable)
                return;

            float yaw = delta.x * model.lookSensitivity.x;
            float pitch = delta.y * model.lookSensitivity.y;
            
            TryRotate(yaw, pitch);
        }
    }
}

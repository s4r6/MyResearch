using UnityEngine;
using UnityEngine.InputSystem;
using UseCase.Player;

namespace View.Player
{
    public class InputController : MonoBehaviour
    {
        [SerializeField]
        PlayerInput input;
        InputAction move;
        InputAction look;

        public PlayerMoveController usecase;

        void Start()
        {
            move = input.actions["Move"];
            look = input.actions["Look"];
        }

        void Update()
        {
            if (input == null || move == null) return;

            Vector2 direction = move.ReadValue<Vector2>();
            usecase.OnMoveInput(direction);
        }

        void LateUpdate()
        {
            if(input == null || look == null) return;

            Vector2 delta = look.ReadValue<Vector2>();
            usecase.OnLookInput(delta);
        }
    }

}

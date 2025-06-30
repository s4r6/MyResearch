using UnityEngine;
using UnityEngine.InputSystem;
using UseCase.Player;
using UniRx;

namespace View.Player
{
    public class InputController : MonoBehaviour
    {
        [SerializeField]
        PlayerInput input;
        InputAction move;
        InputAction look;

        public Subject<Unit> OnInspectButtonPressed = new();
        public Subject<Unit> OnPickUpButtonPressed = new();
        public Subject<Unit> OnActionButtonPressed = new();
        public Subject<Unit> OnFinishButtonPressed = new();
        public Subject<Unit> OnInteractButtonPressed = new();
        public Subject<Unit> OnDocumentButtonPressed = new();
        //public PlayerMoveController usecase;

        void Start()
        {
            move = input.actions["Move"];
            look = input.actions["Look"];
        }


        public Vector2 GetMoveInput()
        {
            if (input == null || move == null) return Vector2.zero;

            Vector2 direction = move.ReadValue<Vector2>();
            return direction;
        }

        public Vector2 GetLookInput()
        {
            if (input == null || look == null) return Vector2.zero;

            Vector2 delta = look.ReadValue<Vector2>();
            return delta;
        }

        public void OnInspect(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            OnInspectButtonPressed.OnNext(default);
        }

        public void OnPickUp(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            OnPickUpButtonPressed.OnNext(default);
        }

        public void OnAction(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            OnActionButtonPressed.OnNext(default);
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            OnInteractButtonPressed.OnNext(default);
        }

        public void OnFinish(InputAction.CallbackContext context)
        {
            if(!context.performed) return;
            OnFinishButtonPressed.OnNext(default);
        }

        public void OnDocument(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            OnDocumentButtonPressed.OnNext(default);
        }

        public void SwitchActionMapToUI()
        {
            input.SwitchCurrentActionMap("UI");
        }

        public void SwitchActionMapToPlayer()
        {
            input.SwitchCurrentActionMap("Player");
            Debug.Log(input.currentActionMap);
        }
    }

}

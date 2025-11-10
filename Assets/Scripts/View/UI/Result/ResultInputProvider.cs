using UnityEngine;
using UniRx;
using UnityEngine.InputSystem;

namespace View.UI
{
    public class ResultInputProvider : MonoBehaviour
    {
        public Subject<float> OnPageChangeButtonPressed = new();

        public void OnPageChange(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            float delta = context.ReadValue<float>();

            OnPageChangeButtonPressed.OnNext(delta);
        }
    }
}
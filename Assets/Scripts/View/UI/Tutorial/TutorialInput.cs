using Presenter.Tutorial;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialInput : MonoBehaviour, ITutorialInput
{
    public event Action NextRequested;

    // InputActions ‚Ì "Tutorial/Next" ‚©‚çŒÄ‚Ô
    public void OnNext(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        NextRequested?.Invoke();
    }
}

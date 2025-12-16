using System;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

public static class InputActionExtentions
{
    public static IObservable<InputAction.CallbackContext> PerformAsObservable(this InputAction action)
    {
        return Observable.FromEvent<InputAction.CallbackContext>(
            h => action.performed += h,
            h => action.performed -= h
        );
    }
}

using UnityEngine;

namespace Domain.Component
{
    public interface IInteractable
    {
        void Interact();
        bool CanInteract();
    }
}
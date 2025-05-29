using UnityEngine;

namespace Domain.Component
{
    public class DoorComponent : GameComponent, IInteractable
    {
        public bool IsOpen = true;
        public bool IsLock = false;

        public void Open()
        {
            IsOpen = !IsOpen; 
        }

        public bool CanUse()
        {
            return !IsLock;
        }

        public void Interact()
        {
            Open();
        }

        public bool CanInteract()
        {
            return CanUse();
        }
    }
}
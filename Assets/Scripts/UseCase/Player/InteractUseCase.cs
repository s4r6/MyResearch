using Domain.Component;
using JetBrains.Annotations;
using UnityEngine;
using View.Stage;

namespace UseCase.Player
{
    public class InteractUseCase
    {
        IObjectRepository repository;
        InteractView view;

        public InteractUseCase(IObjectRepository repository, InteractView view)
        {
            this.repository = repository;
            this.view = view;
        }

        public bool CanInteract(string objectId)
        {
            var entity = repository.GetById(objectId);
            if (entity == null) return false;

            if (!entity.TryGetComponent<DoorComponent>(out var interactable))
                return false;

            if (!interactable.CanInteract())
                return false;

            return true;
        }

        public void TryInteract(string objectId)
        {
            if(!CanInteract(objectId)) return;

            var entity = repository.GetById(objectId);
            var interactable = entity.GetComponent<DoorComponent>();
            interactable.Interact();

            string action = "";

            

            if (interactable.IsOpen)
                action = "close";
            else
            {
                action = "open";
            }

            view.Interact(objectId ,action);
        }
    }
}
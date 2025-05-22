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

        public void TryInteract(string objectId)
        {
            var entity = repository.GetById(objectId);
            Debug.Log(objectId + ":" + entity);
            if (entity == null) return;

            if (!entity.TryGetComponent<DoorComponent>(out var interactable))
                return;

            if (!interactable.CanInteract())
                return;

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
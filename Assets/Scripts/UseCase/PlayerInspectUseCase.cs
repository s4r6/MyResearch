using Domain.Stage.Object;
using Domain.Player;
using UnityEngine;
using View.Player;
using Infrastructure.Stage.Object;

namespace UseCase.Player
{
    public class PlayerInspectUseCase
    {
        public RaycastController raycast;
        public InspectableObjectRepository objManager;
        public PlayerEntity model;
        
        public PlayerInspectUseCase(PlayerEntity model, RaycastController raycast, InspectableObjectRepository manager)
        {
            this.model = model;
            this.raycast = raycast;
            objManager = manager;
        }

        public void Update()
        {
            GetLookingObjectId();
        }

        public void GetLookingObjectId()
        {
            var name = raycast.TryGetLookedObjectId();
            model.currentLookingObject = name;
        }

        public void TryInspect()
        {
            var id = model.currentLookingObject;
            var obj = objManager.TryGetInspectableObject(id);

            if (obj == null)
                return;


            Debug.Log("InspectObject:" + obj.id);
        }
    }
}
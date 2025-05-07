using Domain.Stage.Object;
using Domain.Player;
using UnityEngine;
using View.Player;
using View.UI;
using Infrastructure.Stage.Object;
using System.Linq;
using System;

namespace UseCase.Player
{
    public class PlayerInspectUseCase
    {
        RaycastController raycast;
        InspectableObjectRepository objManager;
        PlayerEntity model;
        ObjectInfoView view;

        InspectableObject currentInspectObject;

        public PlayerInspectUseCase(PlayerEntity model, RaycastController raycast, InspectableObjectRepository manager, ObjectInfoView view)
        {
            this.model = model;
            this.raycast = raycast;
            objManager = manager;
            this.view = view;
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


        Action OnCompleteInspect;
        public bool TryInspect(Action onComplete)
        {
            OnCompleteInspect = onComplete;

            var id = model.currentLookingObject;
            var obj = objManager.TryGetInspectableObject(id);

            currentInspectObject = obj;

            if (obj == null)
                return false;

            var choiceLabels = obj.choices.Values
                                          .Select(x => x.label)
                                          .ToList();

            Debug.Log("InspectObject:" + obj.id);
            view.StartInspect(obj.name, obj.description, choiceLabels, result => OnEndInspect(result));

            return true;
        }
        
        public void OnEndInspect(string? choiceText)
        {
            view.EndInspect();

            //ここで選択肢を反映させる(フラグオンなど)
            if (choiceText != null)
            {
                currentInspectObject.SelectChoice(choiceText);
            }


            Debug.Log($"{choiceText}を選択");

            OnCompleteInspect?.Invoke();
            OnCompleteInspect = null;
            return;
        }
    }
}
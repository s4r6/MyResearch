using Domain.Stage.Object;
using Domain.Player;
using UnityEngine;
using View.Player;
using View.UI;
using Infrastructure.Stage.Object;
using Infrastructure.Action;
using System.Linq;
using System;

namespace UseCase.Player
{
    public class PlayerInspectUseCase
    {
        InspectableObjectRepository objectRepository;
        ActionRepository actionRepository;
        PlayerEntity model;
        ObjectInfoView view;

        InspectableObject currentInspectObject;

        public PlayerInspectUseCase(PlayerEntity model, InspectableObjectRepository objectRepository, ActionRepository actionRepository, ObjectInfoView view)
        {
            this.model = model;
            this.objectRepository = objectRepository;
            this.actionRepository = actionRepository;
            this.view = view;
        }

        public void Update()
        {
            // SystemUseCaseに移動したため、ここでは何もしない
        }

        Action OnCompleteInspect;
        public bool TryInspect(string objectId, Action onComplete)
        {
            OnCompleteInspect = onComplete;

            var obj = objectRepository.TryGetInspectableObject(objectId);

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
            Debug.Log("OnEndInspect");
            view.EndInspect();

            if (choiceText != null)
            {
                Debug.Log($"[PlayerInspectUseCase] 選択前のselectedChoice: {currentInspectObject.selectedChoice}");
                currentInspectObject.SelectChoice(choiceText);
                Debug.Log($"[PlayerInspectUseCase] 選択後のselectedChoice: {currentInspectObject.selectedChoice}");
                Debug.Log($"[PlayerInspectUseCase] 選択したオブジェクトのID: {currentInspectObject.id}");

                //選択したChoiceのアクションを取得
                var actions = currentInspectObject.selectedChoice.availableActions;
                //アクションを保存
                actionRepository.SaveActionEntities(currentInspectObject.id, actions);
            }

            OnCompleteInspect?.Invoke();
            OnCompleteInspect = null;
            return;
        }
    }
}
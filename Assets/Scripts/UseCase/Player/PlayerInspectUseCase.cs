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
        IObjectRepository repository;
        PlayerEntity model;
        ObjectInfoView view;

        InspectableObject currentInspectObject;

        public PlayerInspectUseCase(PlayerEntity model, ObjectInfoView view, IObjectRepository repository)
        {
            this.model = model;
            this.view = view;
            this.repository = repository;
        }

        public void Update()
        {
            // SystemUseCaseに移動したため、ここでは何もしない
        }

        Action OnCompleteInspect;
        public bool TryInspect(string objectId, Action onComplete)
        {
            OnCompleteInspect = onComplete;

            ObjectEntity obj = repository.LoadObjectEntity(objectId);

            var inspectable = obj as InspectableObject;
            if (inspectable == null)
                return false;

            currentInspectObject = inspectable;

            var choiceLabels = inspectable.choices.Select(x => x.label)
                                            .ToList();

            Debug.Log("InspectObject:" + obj.ObjectId);
            view.StartInspect(obj.DisplayName, obj.Description, choiceLabels, result => OnEndInspect(result));

            return true;
        }
        
        public void OnEndInspect(string? choiceText)
        {
            Debug.Log("OnEndInspect");
            view.EndInspect();

            if (choiceText != null)
            {
                Debug.Log(choiceText);
                Debug.Log($"[PlayerInspectUseCase] 選択前のselectedChoice: {currentInspectObject.selectedChoice}");
                currentInspectObject.SelectChoice(choiceText);
                Debug.Log($"[PlayerInspectUseCase] 選択後のselectedChoice: {currentInspectObject.selectedChoice}");
                Debug.Log($"[PlayerInspectUseCase] 選択したオブジェクトのID: {currentInspectObject.ObjectId}");

                //選択したChoiceのアクションを取得
                var actions = currentInspectObject.selectedChoice;
                //アクションを保存
                //actionRepository.SaveActionEntities(currentInspectObject.id);
            }

            OnCompleteInspect?.Invoke();
            OnCompleteInspect = null;
            return;
        }
    }
}
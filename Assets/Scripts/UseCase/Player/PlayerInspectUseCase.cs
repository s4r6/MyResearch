using Domain.Stage.Object;
using Domain.Player;
using UnityEngine;
using View.Player;
using View.UI;
using System.Linq;
using System;
using Domain.Component;
using Unity.VisualScripting;
using static UnityEngine.EventSystems.EventTrigger;
using Domain.Action;

namespace UseCase.Player
{
    public class PlayerInspectUseCase
    {
        IObjectRepository repository;
        PlayerEntity model;
        ObjectInfoView view;

        ObjectEntity currentInspectObject;

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

            ObjectEntity obj = repository.GetById(objectId);
            if (obj == null)
                return false;


            if(obj.TryGetComponent<InspectableComponent>(out var inspectable))
            {
                var choiceLabels = inspectable.Choices.Select(x => x.Label).ToList();
                view.StartInspect(inspectable.DisplayName, inspectable.Description, choiceLabels, result => OnEndInspect(result));
            }
            currentInspectObject = obj;

            Debug.Log("InspectObject:" + obj.Id);
            

            return true;
        }
        
        public void OnEndInspect(string? choiceText)
        {
            Debug.Log("OnEndInspect");
            view.EndInspect();

            if (choiceText != null)
            {
                if (!currentInspectObject.TryGetComponent<InspectableComponent>(out var inspectable)) return;

                var choice = inspectable.Choices.Find(x => x.Label == choiceText);
                if(choice == null) return;
                inspectable.SelectedChoice = choice;
                foreach(var attr in choice.ObjectAttributes)
                {
                    currentInspectObject.AddPermanentAttribute(attr);
                }
                

                // ActionAttributeを動的にActionableComponentに追加（※保持されていない場合は生成）
                if (choice.ActionAttributes.Any())
                {
                    //ActionableComponentがついていないオブジェクトなら
                    if (!currentInspectObject.TryGetComponent<ActionableComponent>(out var actionable))
                    {
                        actionable = new ActionableComponent();
                        currentInspectObject.Add(actionable);
                    }

                    //選択したChoiceに設定されている属性をComponentに追加
                    foreach (var attr in choice.ActionAttributes)
                    {
                        var actionAttr = ActionAttributeFactory.CreateFromName(attr.Name, attr.Target); // "Shredderable" → ActionAttribute
                        actionable.Attributes.Add(actionAttr);
                    }
                }
            }

            OnCompleteInspect?.Invoke();
            OnCompleteInspect = null;
            return;
        }
    }
}
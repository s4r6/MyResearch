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
using Cysharp.Threading.Tasks;

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

        Action OnCompleteInspect;
        public bool TryInspect(string objectId, Action onComplete)
        {
            OnCompleteInspect = onComplete;

            ObjectEntity obj = repository.GetById(objectId);
            if (obj == null)
                return false;


            if (!obj.TryGetComponent<InspectableComponent>(out var inspectable)) return false;
            
            //Choice‚ª‚ ‚ê‚Î
            if(obj.TryGetComponent<ChoicableComponent>(out var choicable))
            {
                var choiceLabels = choicable.Choices.Select(x => x.Label).ToList();
                var index = choiceLabels.FindIndex(x => x == choicable?.SelectedChoice?.Label);
                index = index < 0 ? 0 : index;

                if (!inspectable.IsActioned)
                    view.StartInspect(inspectable.DisplayName, inspectable.Description, index, choiceLabels, result => OnEndInspect(result)).Forget();
                else
                    view.DisplayLabels(inspectable.DisplayName, inspectable.Description, index, choiceLabels, result => OnEndInspect(result)).Forget();

                currentInspectObject = obj;



                return true;
            }
            else
            {
                view.DisplayDescribe(inspectable.DisplayName, inspectable.Description, result => OnEndInspect(result)).Forget();
                return true;
            }
            
        }
        
        public void OnEndInspect(string? choiceText)
        {
            view.EndInspect();

            if (choiceText != null)
            {
                if (!currentInspectObject.TryGetComponent<InspectableComponent>(out var inspectable)) return;
                if (!currentInspectObject.TryGetComponent<ChoicableComponent>(out var choicable)) return;

                var choice = choicable.Choices.Find(x => x.Label == choiceText);
                if(choice == null) return;

                if(!inspectable.IsActioned)
                    choicable.SelectedChoice = choice;

                if (choicable.SelectedChoice.OverrideActions.Any(a => a.target == TargetType.Self))
                    currentInspectObject.Add<ActionSelf>(new ActionSelf());
            }

            OnCompleteInspect?.Invoke();
            OnCompleteInspect = null;
            return;
        }
    }
}
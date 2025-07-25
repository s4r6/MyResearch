using Domain.Component;
using System.Collections.Generic;
using Domain.Stage.Object;
using NUnit.Framework;
using UnityEngine;
using System.Linq;
using Domain.Stage;
using UseCase.Player;

namespace Domain.Action
{
    public class ActionService
    {
        public bool CanAction(ObjectEntity target, ObjectEntity held)
        {
            if(target.TryGetComponent<ActionHeld>(out var actionHeld) && held != null)
            {
                //Action�\���m�F
                if(actionHeld.IsMatch(held) && !(held.TryGetComponent<InspectableComponent>(out var inspectable) && inspectable.IsActioned))
                {
                    return true;
                }
            }

            if (target.TryGetComponent<ActionSelf>(out var actionSelf))
            {
                if (actionSelf.IsMatch(target) && !(target.TryGetComponent<InspectableComponent>(out var targetInsp) && targetInsp.IsActioned))
                {
                    return true;
                }
            }

            return false;
        }

        //���s�\�ȃA�N�V�����̃��X�g���擾
        public List<ActionEntity> GetAvailableActions(ObjectEntity target, ObjectEntity held) 
        {
            List<ActionEntity> availebleActions = new();

            if (target.TryGetComponent<ActionHeld>(out var actionHeld) && held != null)
            {       
                var isMatch = actionHeld.IsMatch(held);
                if (held.TryGetComponent<InspectableComponent>(out var inspectable))
                {
                    if (inspectable.IsActioned)
                        isMatch = false;
                }


                if (isMatch)
                    availebleActions.AddRange(actionHeld?.GetAvailableActions(held));
            }

            if (target.TryGetComponent<ActionSelf>(out var actionSelf) && target != null)
            {
                var isMatch = actionSelf.IsMatch(target);
                if (target.TryGetComponent<InspectableComponent>(out var inspectable))
                {
                    if (inspectable.IsActioned)
                        isMatch = false;
                }


                if (isMatch)
                    availebleActions.AddRange(actionSelf?.GetAvailableActions(target));
            }

            return availebleActions;
        }

        //�I�������A�N�V������K�p
        public bool ApplyAction(List<ActionEntity> actions, string selectedActionLabel, ObjectEntity entity, StageEntity stage)
        {
            
            var selectedAction = actions.Find(a => a.label == selectedActionLabel);
            if(selectedAction == null)
            {
                Debug.Log("Action�����݂��܂���");
                return false;
            }

            if (!CanApplyAction(selectedAction, stage))
            {
                return false;
            }

            SetActionFlag(entity);


            if(!entity.TryGetComponent<ChoicableComponent>(out var choicable))
            {
                Debug.LogError("Choice������܂���");
                return false;
            }
            var selectedRiskLabel = choicable.SelectedChoice.Label;
            var history = new ActionHistory(entity.Id, selectedRiskLabel, selectedActionLabel, selectedAction.riskChange, selectedAction.actionPointCost);
            //�A�N�V������Stage�ɓK�p���ė�����ۑ�
            stage.OnExecuteAction(history);

            return true;
        }

        public bool CanApplyAction(ActionEntity action, StageEntity stage)
        {
            return stage.GetActionPoint() > action.actionPointCost;
        }

        void SetActionFlag(ObjectEntity entity)
        {
            if (!entity.TryGetComponent<InspectableComponent>(out var inspectable))
                return;

            if (inspectable.IsActioned)
                return;

            inspectable.IsActioned = true;
        }
    }
}
using UnityEngine;
using Infrastructure.Stage.Object;
using Domain.Player;
using View.UI;
using Infrastructure.Action;
using System.Linq;
using Domain.Stage.Object;
using System.Collections.Generic;
using Domain.Action;
using System;
using View.Player;
using JetBrains.Annotations;
using Unity.VisualScripting;


namespace UseCase.Player
{
    public class PlayerActionUseCase
    {
        PlayerEntity playerEntity;
        ActionOverlayView actionOverlayView;
        ActionRuleService actionRule;
        PlayerActionExecuter executer;
        IObjectRepository repository;

        Action<ActionEntity> OnCompleteAction;

        List<ActionEntity> cashActions;

        public PlayerActionUseCase(PlayerEntity playerEntity, ActionOverlayView actionOverlayView, ActionRuleService actionRule, PlayerActionExecuter executer, IObjectRepository repository)
        {
            this.playerEntity = playerEntity;
            this.repository = repository;
            this.actionOverlayView = actionOverlayView;
            this.actionRule = actionRule;
            this.executer = executer;
            Debug.Log("[PlayerActionUseCase] 初期化完了");
        }

        public bool TryAction(string objectId, Action<ActionEntity> onComplete)
        {
            OnCompleteAction = onComplete;

            Debug.Log($"[PlayerActionUseCase] TryAction開始: objectId={objectId}");

            if (!IsCanAction(objectId)) return false;

            var obj = repository.LoadObjectEntity(objectId);
            var inspectable = obj as InspectableObject;

            if(inspectable == null)
            {
                var heldItem = repository.LoadObjectEntity(playerEntity.currentCarringObject);
                if (heldItem != null) 
                {
                    var inspectableItem = heldItem as InspectableObject;
                    if (inspectableItem == null) return false;

                    if (!inspectableItem.availableActionIds.Contains("ShredderUse"))
                        return false;

                    cashActions = inspectableItem.selectedChoice.OverrideActions.FindAll(x => x.id == "ShredderUse");
                    actionOverlayView.StartSelectAction(inspectableItem.availableActionIds, objectId, result => OnEndSelectAction(result));
                }

            }
            else
            {
                if(inspectable.availableActionIds.Contains("ShredderUse") || inspectable.availableActionIds.Contains("TrashBin"))
                {
                    var actionIds = inspectable.availableActionIds.FindAll(x => !x.Equals("ShredderUse") && !x.Equals("TrashBin"));
                    if (actionIds.Count <= 0)
                        return false;
                }
                cashActions = inspectable.selectedChoice.OverrideActions;
                actionOverlayView.StartSelectAction(inspectable.availableActionIds, objectId, result => OnEndSelectAction(result));
            }



                return true;
        }

        bool IsCanAction(string lookingObjectId)
        {
            if (lookingObjectId == "")
                return false;

            var obj = repository.LoadObjectEntity(lookingObjectId);
            //そもそも可能なアクションがない
            if(obj.availableActionIds.Count == 0) return false;

            var inspectable = obj as InspectableObject;
            if (inspectable == null)
            {
                if (obj.availableActionIds.Contains("ShredderUse"))
                    return true;

                return false;
            }
            if (inspectable.selectedChoice == null) return false;

            return true;
        }

        void OnEndSelectAction(int? selectedAction)
        {
            actionOverlayView.EndSelectAction();

            if (selectedAction != null) 
            {
                Debug.Log(cashActions[selectedAction.Value].id);

                executer.ActionExecute(cashActions[selectedAction.Value].id, playerEntity.currentCarringObject);
                OnCompleteAction?.Invoke(cashActions[selectedAction.Value]);
            }
            else
            {
                OnCompleteAction?.Invoke(null);
            }
            OnCompleteAction = null;
        }
    }
}

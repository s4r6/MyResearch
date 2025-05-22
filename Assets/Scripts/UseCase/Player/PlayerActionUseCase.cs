using UnityEngine;
using Domain.Player;
using View.UI;
using System.Linq;
using Domain.Stage.Object;
using System.Collections.Generic;
using Domain.Action;
using System;
using View.Player;
using JetBrains.Annotations;
using Unity.VisualScripting;
using static UnityEngine.GraphicsBuffer;
using Domain.Component;


namespace UseCase.Player
{
    public class PlayerActionUseCase
    {
        PlayerEntity playerEntity;
        ActionOverlayView actionOverlayView;
        PlayerActionExecuter playerexecuter;
        IObjectRepository repository;

        Action<(ActionEntity, ObjectEntity)> OnCompleteAction;

        List<ActionEntity> cashActions;
        List<ObjectEntity> cashEntities;

        public PlayerActionUseCase(PlayerEntity playerEntity, ActionOverlayView actionOverlayView, PlayerActionExecuter executer, IObjectRepository repository)
        {
            this.playerEntity = playerEntity;
            this.repository = repository;
            this.actionOverlayView = actionOverlayView;
            this.playerexecuter = executer;
            Debug.Log("[PlayerActionUseCase] 初期化完了");
        }

        public bool TryAction(string objectId, Action<(ActionEntity, ObjectEntity)> onComplete)
        {
            OnCompleteAction = onComplete;

            Debug.Log($"[PlayerActionUseCase] TryAction開始: objectId={objectId}");

            //見ているオブジェクトのEntity取得
            ObjectEntity target = repository.GetById(objectId);
            if (target == null) return false;

            //手に持っているオブジェクトのEntity取得
            ObjectEntity heldItem = repository.GetById(playerEntity.currentCarringObject);

            List<ActionEntity> availebleActions = new();

            var HasActions = false;
            if (target.TryGetComponent<ActionHeld>(out var actionHeld))
            {
                var isMatch = actionHeld.IsMatch(heldItem);
                HasActions = HasActions || isMatch;

                if(isMatch)
                    availebleActions.AddRange(actionHeld?.GetAvailableActions(heldItem));
            }

            if (target.TryGetComponent<ActionSelf>(out var actionSelf)) 
            {
                var isMatch = actionSelf.IsMatch(target);
                HasActions = HasActions || isMatch;

                if(isMatch)
                    availebleActions.AddRange(actionSelf?.GetAvailableActions(target));
            }


            if (!HasActions)
                return false;


            cashActions = availebleActions;

            var actionLabels = cashActions.Select(action => action.label).ToList();
            actionOverlayView.StartSelectAction(actionLabels, objectId, result => OnEndSelectAction(result));
            return true;
        }


        void OnEndSelectAction(int? selectedAction)
        {
            actionOverlayView.EndSelectAction();

            if (selectedAction != null) 
            {
                Debug.Log(cashActions[selectedAction.Value].id);

                

                var actionEntity = cashActions[selectedAction.Value];
                if (actionEntity.target == TargetType.Self)
                {
                    playerexecuter.ActionExecute(cashActions[selectedAction.Value].id, playerEntity.currentLookingObject);
                    var target = repository.GetById(playerEntity.currentLookingObject);
                    OnCompleteAction?.Invoke((actionEntity, target));
                }
                else if(actionEntity.target == TargetType.HeldItem)
                {
                    playerexecuter.ActionExecute(cashActions[selectedAction.Value].id, playerEntity.currentCarringObject);
                    //手に持っているオブジェクトのEntity取得
                    ObjectEntity heldItem = repository.GetById(playerEntity.currentCarringObject);
                    OnCompleteAction?.Invoke((actionEntity, heldItem));
                }
                
            }
            else
            {
                OnCompleteAction?.Invoke((null, null));
            }
            OnCompleteAction = null;
        }
    }
}

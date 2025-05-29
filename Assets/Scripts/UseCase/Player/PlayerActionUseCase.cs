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
using Domain.Stage;


namespace UseCase.Player
{
    public class PlayerActionUseCase
    {
        PlayerEntity playerEntity;
        ActionOverlayView actionOverlayView;
        PlayerActionExecuter executor;
        IObjectRepository repository;
        StageEntity stage;

        Action<(ActionEntity, ObjectEntity)> OnCompleteAction;

        List<ActionEntity> cashActions;

        public PlayerActionUseCase(PlayerEntity playerEntity, ActionOverlayView actionOverlayView, PlayerActionExecuter executor, IObjectRepository repository, StageEntity stage)
        {
            this.playerEntity = playerEntity;
            this.repository = repository;
            this.actionOverlayView = actionOverlayView;
            this.executor = executor;
            this.stage = stage;
        }

        public bool TryAction(string objectId, Action<(ActionEntity, ObjectEntity)> onComplete)
        {
            OnCompleteAction = onComplete;

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
                if (heldItem.TryGetComponent<InspectableComponent>(out var inspectable))
                {
                    if(inspectable.IsActioned)
                        isMatch = false;
                }


                HasActions = HasActions || isMatch;

                if(isMatch)
                    availebleActions.AddRange(actionHeld?.GetAvailableActions(heldItem));
            }

            if (target.TryGetComponent<ActionSelf>(out var actionSelf)) 
            {
                var isMatch = actionSelf.IsMatch(target);
                if (target.TryGetComponent<InspectableComponent>(out var inspectable))
                {
                    if (inspectable.IsActioned)
                        isMatch = false;
                }


                HasActions = HasActions || isMatch;

                if (isMatch)
                    availebleActions.AddRange(actionSelf?.GetAvailableActions(target));
            }


            if (!HasActions)
                return false;


            cashActions = availebleActions;

            var actions = cashActions.Select(action => (action.label, action.actionPointCost)).ToList();
            actionOverlayView.StartSelectAction(stage.GetActionPoint(), stage.maxActionPoint, actions, objectId, result => OnEndSelectAction(result));
            return true;
        }


        void OnEndSelectAction(int? selectedAction)
        {
            //何も選択しなければ、終了
            if (selectedAction == null)
            {
                actionOverlayView.EndSelectAction();
                OnCompleteAction?.Invoke((null, null));
                OnCompleteAction = null;
                return;
            }

            var actionEntity = cashActions[selectedAction.Value];
            //アクションコストが現在のアクションポイントより多いと
            if (stage.GetActionPoint() < actionEntity.actionPointCost)
            {
                Debug.Log("ActionPointが足りません");
                actionOverlayView.OutPutLog();
                OnCompleteAction?.Invoke((null, null));
                OnCompleteAction = null;
                return;
            }

            actionOverlayView.EndSelectAction();
            

            if (actionEntity.target == TargetType.Self)
            {
                executor.ActionExecute(cashActions[selectedAction.Value].id, "", playerEntity.currentLookingObject);
                var target = repository.GetById(playerEntity.currentLookingObject);
                SetActionFlag(target);
                OnCompleteAction?.Invoke((actionEntity, target));
            }
            else if(actionEntity.target == TargetType.HeldItem)
            {
                executor.ActionExecute(cashActions[selectedAction.Value].id, playerEntity.currentCarringObject, playerEntity.currentLookingObject);
                //手に持っているオブジェクトのEntity取得
                ObjectEntity heldItem = repository.GetById(playerEntity.currentCarringObject);
                SetActionFlag(heldItem);
                OnCompleteAction?.Invoke((actionEntity, heldItem));
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
}

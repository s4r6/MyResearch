using UnityEngine;
using Domain.Player;
using View.UI;
using System.Linq;
using Domain.Stage.Object;
using System.Collections.Generic;
using Domain.Action;
using System;
using View.Player;
using Domain.Component;
using Domain.Stage;
using Unity.VisualScripting;
using static UnityEngine.GraphicsBuffer;
using Cysharp.Threading.Tasks;


namespace UseCase.Player
{
    public interface IActionPresenter
    {
        void StartSelectAction(int remainingActionPoint, int maxActionPoint, List<(string, int)> actions, string targetObjectId, Action<string> onEnd);
        void EndSelectAction();
        void OutPutLog();
    }

    public class PlayerActionUseCase : IActionUseCase
    {
        PlayerEntity playerEntity;
        IActionPresenter presenter;
        PlayerActionExecuter executor;
        IObjectRepository repository;
        StageEntity stage;
        ActionService actionService;

        Action OnCompleteAction;

        List<ActionEntity> cashActions;

        public PlayerActionUseCase(PlayerEntity playerEntity, IActionPresenter presenter, PlayerActionExecuter executor, IObjectRepository repository, StageEntity stage, ActionService actionService)
        {
            this.playerEntity = playerEntity;
            this.repository = repository;
            this.presenter = presenter;
            this.executor = executor;
            this.stage = stage;
            this.actionService = actionService;
        }

        public bool TryAction(string objectId, Action onComplete)
        {
            OnCompleteAction = onComplete;

            //見ているオブジェクトのEntity取得
            ObjectEntity target = repository.GetById(objectId);
            if (target == null) return false;

            //手に持っているオブジェクトのEntity取得
            ObjectEntity heldItem = repository.GetById(playerEntity.currentCarringObject);

            var availableActions = actionService.GetAvailableActions(target, heldItem);
            if(availableActions == null) return false;

            var actions = availableActions.Select(action => (action.label, action.actionPointCost)).ToList();
            if(actions.Count == 0) return false;
            presenter.StartSelectAction(stage.GetActionPoint(), stage.maxActionPoint, actions, objectId, result => OnEndSelectAction(result));

            cashActions = availableActions;
            return true;
        }


        public UniTask OnEndSelectAction(string selectedActionLabel)
        {
            //バリデーションチェック
            if (!string.IsNullOrEmpty(selectedActionLabel)) 
            {
                var actionEntity = cashActions.Find(a => a.label == selectedActionLabel);
                var actionId = actionEntity.id;

                if (actionEntity.target == TargetType.Self)
                {
                    var target = repository.GetById(playerEntity.currentLookingObject);
                    var result = actionService.ApplyAction(cashActions, selectedActionLabel, target, stage);
                    if (result)
                    {
                        executor.ActionExecute(actionId, "", target.Id);
                    }
                    else
                    {
                        presenter.OutPutLog();
                    }
                        
                }
                else if (actionEntity.target == TargetType.HeldItem)
                {
                    var heldItem = repository.GetById(playerEntity.currentCarringObject);
                    var result = actionService.ApplyAction(cashActions, selectedActionLabel, heldItem, stage);
                    if (result)
                    {
                        executor.ActionExecute(actionId, playerEntity.currentCarringObject, playerEntity.currentLookingObject);
                    }
                    else
                    {
                        presenter.OutPutLog();
                    }
                }
            }

            OnCompleteAction?.Invoke();
            OnCompleteAction = null;

            return UniTask.CompletedTask;
        }
    }
}

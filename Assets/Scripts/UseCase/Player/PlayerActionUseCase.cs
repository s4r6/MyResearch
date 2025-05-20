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


namespace UseCase.Player
{
    public class PlayerActionUseCase
    {
        PlayerEntity playerEntity;
        ActionOverlayView actionOverlayView;
        ActionExecuter executor;
        PlayerActionExecuter playerexecuter;
        IObjectRepository repository;

        Action<ActionEntity> OnCompleteAction;

        List<ActionEntity> cashActions;

        public PlayerActionUseCase(PlayerEntity playerEntity, ActionOverlayView actionOverlayView, ActionExecuter executor, PlayerActionExecuter executer, IObjectRepository repository)
        {
            this.playerEntity = playerEntity;
            this.repository = repository;
            this.actionOverlayView = actionOverlayView;
            this.playerexecuter = executer;
            this.executor = executor;
            Debug.Log("[PlayerActionUseCase] 初期化完了");
        }

        public bool TryAction(string objectId, Action<ActionEntity> onComplete)
        {
            OnCompleteAction = onComplete;

            Debug.Log($"[PlayerActionUseCase] TryAction開始: objectId={objectId}");

            //見ているオブジェクトのEntity取得
            var target = repository.GetById(objectId);
            if (target == null) return false;

            //手に持っているオブジェクトのEntity取得
            ObjectEntity heldItem = repository.GetById(playerEntity.currentCarringObject);

            //ActionComponentに設定されているAttributeを取得
            var matched = executor.Evaluate(target, heldItem);
            if (matched.Count == 0)
            {
                return false;
            }

            //

            //actionOverlayView.StartSelectAction(action)
            return true;
        }


        void OnEndSelectAction(int? selectedAction)
        {
            actionOverlayView.EndSelectAction();

            if (selectedAction != null) 
            {
                Debug.Log(cashActions[selectedAction.Value].id);


                //手に持っているオブジェクトのEntity取得
                ObjectEntity heldItem = repository.GetById(playerEntity.currentCarringObject);

                //executor.ExecuteMatced(, heldItem);
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

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


namespace UseCase.Player
{
    public class PlayerActionUseCase
    {
        PlayerEntity playerEntity;
        InspectableObjectRepository objectRepository;
        ActionRepository actionRepository;
        ActionOverlayView actionOverlayView;
        ActionRuleService actionRule;
        PlayerActionExecuter executer;

        Action OnCompleteAction;

        public PlayerActionUseCase(PlayerEntity playerEntity, InspectableObjectRepository objectRepository, ActionRepository actionRepository, ActionOverlayView actionOverlayView, ActionRuleService actionRule, PlayerActionExecuter executer)
        {
            this.playerEntity = playerEntity;
            this.objectRepository = objectRepository;
            this.actionRepository = actionRepository;
            this.actionOverlayView = actionOverlayView;
            this.actionRule = actionRule;
            this.executer = executer;
            Debug.Log("[PlayerActionUseCase] 初期化完了");
        }

        //ObjectIdをもつオブジェクトに対して現在可能なアクションのListを返す
        /*public List<ActionEntity> FindAvailableActionId(string objectId)
        {
            //Objectがデータ上可能なアクションidのリストを取得
            var availableActionIds = repository.TryGetAvailableActionIds(objectId);
            //可能なアクションがない場合は何もしない
            if(availableActionIds == null)
            {
                return null;
            }

            //フラグが立っているアクションのリストを取得
            var availableActions = repository.TryGetAvailableActions(objectId);
            foreach(var actions in availableActions)
            {
                //actions = List<ActionEntity>
                //availableActionIds = List<string>
                foreach(var actionId in availableActionIds)
                {
                    var availableAction = actions.Find(x => x.id == actionId);
                    if(availableAction != null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }*/

        public bool TryAction(string objectId, Action onComplete)
        {
            OnCompleteAction = onComplete;

            Debug.Log($"[PlayerActionUseCase] TryAction開始: objectId={objectId}");
            //視認しているオブジェクトを使うアクションを持つオブジェクトのリストを取得
            var actionEntities = actionRepository.TryGetActionableObjectsById(objectId);
            Debug.Log($"[PlayerActionUseCase] 取得したアクションエンティティ数: {(actionEntities != null ? actionEntities.Count : 0)}");

            //フラグが立っていない or 視認しているオブジェクトはAction対象でない
            if(actionEntities == null)
            {
                Debug.Log($"[PlayerActionUseCase] アクション可能なエンティティが見つかりません: objectId={objectId}");
                return false;
            }

            List<ActionEntity> ActionSelf = new();
            List<ActionEntity> ActionNeedOther = new();

            foreach(var actionEntity in actionEntities)
            {
                Debug.Log($"[PlayerActionUseCase] アクションエンティティ処理中: {actionEntity.id}");
                var actions = actionEntity.selectedChoice.availableActions;
                Debug.Log($"[PlayerActionUseCase] 利用可能なアクション数: {actions.Count}");
                
                foreach(var action in actions)
                {
                    Debug.Log($"[PlayerActionUseCase] アクション処理中: {action.id}, 実行対象: {action.executeOnObjectId}, ターゲット: {action.targetObjectId}");
                    //自分を対象にするアクション
                    if(action.executeOnObjectId == action.targetObjectId)
                    {
                        ActionSelf.Add(action);
                        Debug.Log($"[PlayerActionUseCase] 自己アクション追加: {action.id}");
                    }
                    //他のオブジェクトを対象にするアクション
                    else
                    {
                        ActionNeedOther.Add(action);
                        Debug.Log($"[PlayerActionUseCase] 他者アクション追加: {action.id}");
                    }
                }
            }

            Debug.Log($"[PlayerActionUseCase] 自己アクション数: {ActionSelf.Count}, 他者アクション数: {ActionNeedOther.Count}");

            //自分を対象にするアクションは表示
            List<ActionEntity> availableActions = new(ActionSelf);
            //他のオブジェクトを対象にするアクションは、持っているオブジェクトが該当するなら表示
            var actionNeedOther = ActionNeedOther.Find(x => x.targetObjectId == playerEntity.currentCarringObject);
            if(actionNeedOther != null)
            {
                Debug.Log($"[PlayerActionUseCase] 他者アクション追加: {actionNeedOther.id}, 現在持っているオブジェクト: {playerEntity.currentCarringObject}");
                availableActions.Add(actionNeedOther);
            }

            //アクションの表示
            var actionLabels = availableActions.Select(x => x.label).ToList();
            if(actionLabels.Count <= 0)
            {
                Debug.Log($"[PlayerActionUseCase] 表示するアクションがありません: objectId={objectId}");
                return false;
            }

            Debug.Log($"[PlayerActionUseCase] 表示するアクション: {string.Join(", ", actionLabels)}");
            actionOverlayView.StartSelectAction(actionLabels, objectId, result => OnEndSelectAction(result));

            return true;
        }

        void OnEndSelectAction(string? selectedAction)
        {
            actionOverlayView.EndSelectAction();

            if (selectedAction != null) 
            {
                Debug.Log(selectedAction);
                var actionId = actionRepository.TryGetActionIdByLabel(selectedAction);
                if(actionId == null)
                {
                    Debug.LogError("ActionIdがありません");
                    return;
                }

                string executeObjectId = playerEntity.currentLookingActionableObjectId;
                string targetObjectId = playerEntity.currentCarringObject;

                //アクションを実行
                var result = actionRule.Execute(actionId, executeObjectId, targetObjectId);
                Debug.Log($"アクション結果{result.result}");

                executer.ActionExecute(result.result, result.executeObjectId, result.targetObjectId);
            }

            OnCompleteAction?.Invoke();
            OnCompleteAction = null;
        }
    }
}

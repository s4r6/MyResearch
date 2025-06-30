using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Domain.Action;
using Domain.Player;
using Domain.Stage;
using Domain.Stage.Object;
using UnityEditor.SceneManagement;
using UnityEngine;
using UseCase.Network;
using View.Player;

namespace UseCase.Player
{
    public class RemoteActionUseCase : IActionUseCase
    {
        PlayerEntity playerEntity;
        IActionPresenter presenter;
        PlayerActionExecuter executor;
        IObjectRepository repository;
        StageEntity stage;
        ActionService actionService;
        
        IPacketSender sender;
        ISessionRepository sessionRepository;

        Action OnCompleteAction;

        List<ActionEntity> cashActions;

        public RemoteActionUseCase(PlayerEntity playerEntity, IActionPresenter presenter, PlayerActionExecuter executor, IObjectRepository repository, StageEntity stage, ActionService actionService, IPacketSender sender, ISessionRepository sessionRepository)
        {
            this.playerEntity = playerEntity;
            this.repository = repository;
            this.presenter = presenter;
            this.executor = executor;
            this.stage = stage;
            this.actionService = actionService;
            this.sender = sender;
            this.sessionRepository = sessionRepository;
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
            if (availableActions == null) return false;

            var actions = availableActions.Select(action => (action.label, action.actionPointCost)).ToList();
            if (actions.Count == 0) return false;
            presenter.StartSelectAction(stage.GetActionPoint(), stage.maxActionPoint, actions, objectId, result => OnEndSelectAction(result));

            cashActions = availableActions;
            return true;
        }

        public async UniTask OnEndSelectAction(string selectedActionLabel)
        {
            var roomSession = sessionRepository.GetRoomSession();
            var playerSession = sessionRepository.GetPlayerSession();
            if(roomSession == null || playerSession == null)
            {
                throw new Exception("RoomSessionまたはPlayerSessionがありません");
            }

            if (string.IsNullOrEmpty(selectedActionLabel))
                return;

            var playerId = playerSession.Id;
            var roomId = roomSession.Id;

            var actionEntity = cashActions.Find(a => a.label == selectedActionLabel);
            var targetType = actionEntity.target;

            var dto = new ActionResultData
            {
                PlayerId = playerId,
                RoomId = roomId,
                ObjectId = targetType switch
                {
                    TargetType.Self => playerEntity.currentLookingObject,
                    TargetType.HeldItem => playerEntity.currentCarringObject
                },
                SelectedActionLabel = selectedActionLabel
            };

            var result = await sender.SendActionData(dto);
            if(result == ActionResultType.ShortageActionPoint)
            {
                Debug.LogError("ActionPointが足りません");
                presenter.OutPutLog();
                return;
            }

            if(result == ActionResultType.Success)
            {
                switch(targetType)
                {
                    case TargetType.Self:
                        executor.ActionExecute(actionEntity.id, "", playerEntity.currentLookingObject);
                        break;
                    case TargetType.HeldItem:
                        executor.ActionExecute(actionEntity.id, playerEntity.currentCarringObject, playerEntity.currentLookingObject);
                        break;
                }
            }

            OnCompleteAction?.Invoke();
            OnCompleteAction = null;

            return;
        }

        
    }
}
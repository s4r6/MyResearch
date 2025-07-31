using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Domain.Action;
using Domain.Player;
using Domain.Stage;
using Domain.Stage.Object;
using Presenter.Network;
using UniRx;
using UnityEngine;
using UseCase.Network;
using View.Player;

namespace UseCase.Player
{
    public class RemoteActionUseCase : IActionUseCase, IDisposable
    {
        PlayerEntity playerEntity;
        IActionPresenter presenter;
        PlayerActionExecuter executor;
        IObjectRepository repository;
        StageEntity stage;
        ActionService actionService;
        
        IPacketSender sender;
        WebSocketReceiver receiver;
        ISessionRepository sessionRepository;
        IStageRepository stageRepository;

        Action OnCompleteAction;

        List<ActionEntity> cashActions;
        CompositeDisposable _disposables = new();

        public RemoteActionUseCase(PlayerEntity playerEntity, IActionPresenter presenter, PlayerActionExecuter executor, IObjectRepository repository, StageEntity stage, ActionService actionService, IPacketSender sender, WebSocketReceiver receiver, ISessionRepository sessionRepository, IStageRepository stageRepository)
        {
            this.playerEntity = playerEntity;
            this.repository = repository;
            this.presenter = presenter;
            this.executor = executor;
            this.stage = stage;
            this.actionService = actionService;
            
            this.sender = sender;
            this.receiver = receiver;

            this.sessionRepository = sessionRepository;
            this.stageRepository = stageRepository;

            Bind();
        }

        public bool CanAction(string objectId)
        {
            //見ているオブジェクトのEntity取得
            ObjectEntity target = repository.GetById(objectId);
            if (target == null) return false;

            //手に持っているオブジェクトのEntity取得
            ObjectEntity heldItem = repository.GetById(playerEntity.currentCarringObject);

            return actionService.CanAction(target, heldItem);
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
            {
                OnCompleteAction?.Invoke();
                OnCompleteAction = null;
                return;
            }
                

            var playerId = playerSession.Id;
            var roomId = roomSession.Id;

            var actionEntity = cashActions.Find(a => a.label == selectedActionLabel);
            var targetType = actionEntity.target;

            var dto = new ActionRequestData
            {
                PlayerId = playerId,
                RoomId = roomId,
                ObjectId = playerEntity.currentLookingObject,
                HeldId = playerEntity.currentCarringObject,
                Type = targetType,
                SelectedActionLabel = selectedActionLabel
            };

            await sender.SendActionData(dto);

            //ExecuteAction(actionEntity.id, playerEntity.currentCarringObject, playerEntity.currentLookingObject, targetType, result);

            return;
        }

        void ExecuteAction(string actionId, string heldId, string lookingId, TargetType type, ActionResultType result)
        {
            if (result == ActionResultType.ShortageActionPoint)
            {
                Debug.Log("ActionPointが足りません");
                presenter.OutPutLog();
            }
            else if (result == ActionResultType.Success)
            {
                Debug.Log("Action適用");
                switch (type)
                {
                    case TargetType.Self:
                        executor.ActionExecute(actionId, "", lookingId);
                        break;
                    case TargetType.HeldItem:
                        executor.ActionExecute(actionId, heldId, lookingId);
                        break;
                }
            }

            OnCompleteAction?.Invoke();
            OnCompleteAction = null;
        }

        void Bind()
        {
            receiver.OnReceiveActionResultData
                .Subscribe(x =>
                {
                    //RepositoryにAction適用後のEntityを保存
                    if (x.Result == ActionResultType.Success) 
                    {
                        repository.Save(x.ObjectData);
                        var stage = stageRepository.GetCurrentStageEntity();

                        var currentRiskAmount = x.currentRiskAmount;
                        var currentActionPointAmount = x.currentActionPointAmount;
                        var histories = x.histories;

                        stage.Update(currentRiskAmount, currentActionPointAmount, histories);
                    }
                    ExecuteAction(x.ActionId, x.HeldId, x.TargetId, x.Target, x.Result);
                }).AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
        
    }
}
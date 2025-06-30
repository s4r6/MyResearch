using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Domain.Action;
using Domain.Component;
using Domain.Player;
using Domain.Stage.Object;
using JetBrains.Annotations;
using UnityEngine;
using UseCase.Network;
using UseCase.Player;
using View.UI;

namespace UseCase.Player.Network
{
    public class RemoteInspectUseCase : IInspectUseCase
    {
        ISessionRepository sessionRepository;
        IObjectRepository repository;
        IInspectPresenter presenter;
        IPacketSender sender;
        InspectService inspectService;

        ObjectEntity currentInspectObject;

        public RemoteInspectUseCase(IInspectPresenter presenter, InspectService inspectService, IObjectRepository repository, IPacketSender sender, ISessionRepository sessionRepository) 
        { 
            this.sessionRepository = sessionRepository;
            this.sender = sender;
            this.repository = repository;
            this.presenter = presenter;
            this.inspectService = inspectService;   
        }

        Action OnCompleteInspect;
        public bool TryInspect(string objectId, Action onComplete)
        {
            OnCompleteInspect = onComplete;

            ObjectEntity obj = repository.GetById(objectId);

            if (!inspectService.CanInspect(obj)) return false;

            currentInspectObject = obj;

            var choicable = inspectService.TryGetChoice(obj);

            var Inspectable = obj.GetComponent<InspectableComponent>();
            var dto = new InspectData
            {
                DisplayName = Inspectable.DisplayName,
                Description = Inspectable.Description,
                ChoiceLabels = choicable?.Choices?.Select(x => x.Label).ToList() ?? null,
                SelectedLabel = choicable?.SelectedChoice?.Label ?? string.Empty,
                IsSelectable = !Inspectable.IsActioned
            };

            presenter.StartInspect(dto, async result => await OnEndInspect(result)).Forget();

            return true;
        }

        public async UniTask OnEndInspect(string choiceText)
        {
            var roomSession = sessionRepository.GetRoomSession();
            var playerSession = sessionRepository.GetPlayerSession();
            if (roomSession == null || playerSession == null)
            {
                throw new Exception("RoomSessionまたはPlayerSessionがありません");
            }

            if (string.IsNullOrEmpty(choiceText))
            {
                OnCompleteInspect?.Invoke();
                OnCompleteInspect = null;
                return;
            }

            var playerId = playerSession.Id;
            var roomId = roomSession.Id;

            var dto = new InspectResultData
            {
                PlayerId = playerId,
                RoomId = roomId,
                ObjectId = currentInspectObject.Id,
                SelectedChoiceLabel = choiceText,
            };

            var IsSuccess = await sender.SendInspectData(dto);
            if (!IsSuccess)
            {
                Debug.Log("Inspectがサーバー側で失敗しました.");
                return;   
            }

            OnCompleteInspect?.Invoke();
            OnCompleteInspect = null;

            Debug.Log("Inspect終了");
            return;
        }

    }
}
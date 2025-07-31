using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Domain.Network;
using Domain.Stage;
using UniRx;
using UseCase.Network.DTO;
using UseCase.Player;

namespace UseCase.Network
{
    class RoomDiffResult
    {
        public List<RoomSession> Added { get; }
        public List<RoomSession> Removed { get; }
        public List<RoomSession> Updated { get; }

        public RoomDiffResult(List<RoomSession> added, List<RoomSession> removed, List<RoomSession> updated)
        {
            Added = added;
            Removed = removed;
            Updated = updated;
        }
    }
    public class RoomUseCase : IDisposable
    {
        private readonly ISessionRepository sessionRepository;
        readonly IObjectRepository objectRepository;
        readonly IStageRepository stageRepository;
        readonly IPacketReceiver receiver;

        List<RoomSession> lastRooms = new();

        CompositeDisposable _disposables = new();

        public RoomUseCase(ISessionRepository sessionRepository, IObjectRepository objectRepository, IStageRepository stageRepository, IPacketReceiver receiver)
        {
            this.sessionRepository = sessionRepository;
            this.objectRepository = objectRepository;
            this.stageRepository = stageRepository;
            this.receiver = receiver;

            Bind();
        }

        Action<CreateRoomOutputData> OnCompleteCreate;
        public async UniTask Create(CreateRoomInputData input, Action<CreateRoomOutputData> OnComplete)
        {
            OnCompleteCreate = OnComplete;
            await sessionRepository.Create(input.RoomId, input.PlayerName, input.StageId);
        }

        public void OnReceiveCreateResult(CreateRoomResult result)
        {
            //é©êgÇÃSessionÇçÏê¨
            var player = new PlayerSession(result.PlayerId, result.PlayerName);
            sessionRepository.SavePlayerSession(player);

            //RoomÇçÏê¨
            var room = new RoomSession(result.RoomId, result.RoomName, new List<PlayerSession> { player }, result.StageId);
            sessionRepository.SaveRoomSession(room);

            //StageEntityÇçÏê¨
            var stage = new StageEntity(result.MaxRiskAmount, result.MaxActionPointAmount);
            stageRepository.Save(stage);
            
            //ObjectÇRepositoryÇ…ìoò^
            foreach (var entity in result.ObjectData)
            {
                objectRepository.Save(entity);
            }

            var roomData = new CreateRoomOutputData
            {
                Success = true,
                RoomId = result.RoomId,
                StageId = result.StageId,
                ConnectionId = result.PlayerId
            };

            OnCompleteCreate?.Invoke(roomData);
        }

        Action<JoinRoomOutputData> OnCompleteJoin;
        public async UniTask Join(JoinRoomInputData input, Action<JoinRoomOutputData> OnComplete)
        {
            OnCompleteJoin = OnComplete;
            await sessionRepository.Join(input.RoomId, input.PlayerName);
        }

        public void OnReceiveJoinResult(JoinRoomResult result)
        {
            //é©êgÇÃSessionÇçÏê¨
            var player = new PlayerSession(result.PlayerId, result.PlayerName);
            sessionRepository.SavePlayerSession(player);

            //RoomÇçÏê¨
            var room = new RoomSession(result.RoomId, result.RoomName, result.Players, result.StageId);
            sessionRepository.SaveRoomSession(room);

            //StageEntityÇçÏê¨
            var stage = new StageEntity(result.MaxRiskAmount, result.MaxActionPointAmount);
            stageRepository.Save(stage);

            //ObjectÇRepositoryÇ…ìoò^
            foreach (var entity in result.ObjectData)
            {
                objectRepository.Save(entity);
            }

            var data = new JoinRoomOutputData
            {
                Success = true,
                RoomId = result.RoomId,
                StageId = result.StageId
            };
            OnCompleteJoin?.Invoke(data);
        }

        public async UniTask Search(Action<SearchRoomOutputData> OnComplete)
        {
            var newRooms = await sessionRepository.Search();

            var diff = CalcDiff(lastRooms, newRooms);
            lastRooms = newRooms;

            var result = new SearchRoomOutputData
            {
                Added = diff.Added,
                Removed = diff.Removed,
                Updated = diff.Updated,
            };

            OnComplete?.Invoke(result);
        }

        RoomDiffResult CalcDiff(List<RoomSession> oldList, List<RoomSession> newList)
        {
            var oldMap = oldList.ToDictionary(r => r.Id);
            var newMap = newList.ToDictionary(r => r.Id);

            var added = newList.Where(r => !oldMap.ContainsKey(r.Id)).ToList();
            var removed = oldList.Where(r => !newMap.ContainsKey(r.Id)).ToList();

            // çXêVîªíËÅFë∂ç›Ç∑ÇÈÇ™ì‡óeÇ™ïœÇÌÇ¡ÇΩ
            var updated = newList
                .Where(r => oldMap.ContainsKey(r.Id) && !r.Equals(oldMap[r.Id]))
                .ToList();

            return new RoomDiffResult(added, removed, updated);
        }

        void Bind()
        {
            receiver.OnReceiveCreateResponse
                .Subscribe(x =>
                {
                    OnReceiveCreateResult(x);
                }).AddTo(_disposables);

            receiver.OnReceiveJoinResponse
                .Subscribe(x =>
                {
                    OnReceiveJoinResult(x);
                }).AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables.Dispose(); 
        }
    }
}
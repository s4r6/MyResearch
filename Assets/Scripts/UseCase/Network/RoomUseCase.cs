using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Domain.Network;
using Domain.Stage;
using UniRx;
using UseCase.Network.DTO;
using UseCase.Player;

namespace UseCase.Network
{
    public class RoomUseCase : IDisposable
    {
        private readonly ISessionRepository sessionRepository;
        readonly IObjectRepository objectRepository;
        readonly IStageRepository stageRepository;
        readonly IPacketReceiver receiver;

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
            var player = new PlayerSession(result.ConnectionId, result.Name);
            sessionRepository.SavePlayerSession(player);

            //RoomÇçÏê¨
            var room = new RoomSession(result.RoomId, new List<PlayerSession> { player }, result.StageId);
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
                ConnectionId = result.ConnectionId
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
            var player = new PlayerSession(result.ConnectionId, result.Name);
            sessionRepository.SavePlayerSession(player);

            //RoomÇçÏê¨
            var room = new RoomSession(result.RoomId, result.Players, result.StageId);
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
            var roomList = await sessionRepository.Search();

            var roomDatas = new List<(string, int)>();
            foreach (var room in roomList)
            {
                roomDatas.Add((room.Id, room.Players.Count));
            }
            var result = new SearchRoomOutputData
            {
                RoomDatas = roomDatas
            };
            OnComplete?.Invoke(result);
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
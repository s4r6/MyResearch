using System;
using Cysharp.Threading.Tasks;
using Domain.Network;
using Infrastructure.Network;
using Newtonsoft.Json;
using Presenter.Network;
using UnityEngine;
using UseCase.Network.DTO;

namespace UseCase.Network
{
    public class RoomUseCase
    {
        private readonly IRoomRepository roomRepository;

        public RoomUseCase(IRoomRepository roomRepository)
        {
            this.roomRepository = roomRepository;
        }

        public async UniTask Create(CreateRoomInputData input, Action<CreateRoomOutputData, RoomSession> OnComplete)
        {
            var room = await roomRepository.Create(input.RoomId, input.PlayerName, input.StageId);

            var result = new CreateRoomOutputData
            {
                Success = true,
                RoomId = room.Id,
                ConnectionId = room.Players[0].Id
            };
            OnComplete?.Invoke(result, room);
        }

        public void Join(JoinRoomInputData input)
        {

        }
    }
}
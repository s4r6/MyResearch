using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Domain.Network;

namespace UseCase.Network
{
    public interface ISessionRepository
    {
        RoomSession GetRoomSession();
        PlayerSession GetPlayerSession();

        UniTask<RoomSession> Create(string roomId, string playerName, int stageId);
        void Join(string roomId);
        IEnumerable<PlayerSession> GetAllPlayerSession();
    }
}

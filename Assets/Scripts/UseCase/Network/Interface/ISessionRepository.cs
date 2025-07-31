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

        void SavePlayerSession(PlayerSession player);
        void SaveRoomSession(RoomSession room);
        UniTask Create(string roomId, string playerName, int stageId);
        UniTask<List<RoomSession>> Search();
        UniTask Join(string roomId, string playerName);
        IEnumerable<PlayerSession> GetAllPlayerSession();
        void CreatePlayerSession(string id = "", string name = "");
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Domain.Network;

namespace UseCase.Network
{
    public interface IRoomRepository
    {
        RoomSession? FindById(string roomId);
        UniTask<RoomSession> Create(string roomId, string playerName, int stageId);
        void Join(string roomId);
        IEnumerable<RoomSession> GetAll();
    }
}

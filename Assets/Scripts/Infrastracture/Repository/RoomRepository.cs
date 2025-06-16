using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Domain.Network;
using Infrastracture.Network;
using Infrastructure.Network;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Unity.VisualScripting.Antlr3.Runtime;
using UseCase.Network;

namespace Infrastracture.Repository
{
    public class RoomRepository : IRoomRepository
    {
        IWebSocketService server;
        public RoomSession CurrentRoomSession = null;
        public RoomRepository(IWebSocketService server)
        {
            this.server = server;
        }

        public async UniTask<RoomSession> Create(string roomId, string playerName, int stageId)
        {
            var packet = new PacketModel<CreateRoomRequest>
            {
                PacketId = PacketId.CreateRoomRequest,
                Payload = new CreateRoomRequest
                {
                    RoomId = roomId,
                    StageId = stageId
                }
            };

            var response = await server.SendAndReceive(packet, PacketId.CreateRoomResponse);

            if(!response.TryGetValue("Payload", out var payload))
            {
                return null;
            }

            if (!payload["Success"]?.Value<bool>() ?? false)
                throw new Exception("ルーム作成に失敗しました。");

            var player = new PlayerSession(payload["ConnectionId"]?.Value<string>() ?? string.Empty, playerName);

            CurrentRoomSession = new RoomSession(payload["RoomId"]?.Value<string>() ?? string.Empty, new List<PlayerSession> {player});
            return CurrentRoomSession;
        }

        public void Join(string roomId)
        {
            throw new NotImplementedException();
        }

        public RoomSession FindById(string roomId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<RoomSession> GetAll()
        {
            throw new NotImplementedException();
        }

        
    }
}

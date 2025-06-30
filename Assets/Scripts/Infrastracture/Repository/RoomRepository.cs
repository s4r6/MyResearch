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
    public class RoomRepository : ISessionRepository
    {
        IWebSocketService server;

        
        RoomSession CurrentRoomSession = null;
        PlayerSession SelfPlayerSession = null;
        
        Dictionary<string, PlayerSession> RemotePlayerSessions = new Dictionary<string, PlayerSession>();

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
            SelfPlayerSession = player;

            CurrentRoomSession = new RoomSession(payload["RoomId"]?.Value<string>() ?? string.Empty, new List<PlayerSession> {player});
            return CurrentRoomSession;
        }

        public void Join(string roomId)
        {
            throw new NotImplementedException();
        }

        public RoomSession GetRoomSession()
        {
            return CurrentRoomSession;
        }

        public PlayerSession GetPlayerSession()
        {
            return SelfPlayerSession;
        }

        public IEnumerable<RoomSession> GetAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PlayerSession> GetAllPlayerSession()
        {
            throw new NotImplementedException();
        }
    }
}

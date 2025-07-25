using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using Domain.Network;
using Infrastracture.Network;
using Infrastructure.Network;
using UseCase.Network;

namespace Infrastracture.Repository
{
    public class RoomRepository : ISessionRepository
    {
        IWebSocketService server;
        
        RoomSession CurrentRoomSession = null;
        PlayerSession SelfPlayerSession = null;

        public RoomRepository(IWebSocketService server)
        {
            this.server = server;
        }

        public async UniTask Create(string roomId, string playerName, int stageId)
        {
            var packet = new PacketModel<CreateRoomRequest>
            {
                PacketId = PacketId.CreateRoomRequest,
                Payload = new CreateRoomRequest
                {
                    RoomId = roomId,
                    StageId = stageId,
                    PlayerId = playerName
                }
            };

            await server.Send(packet);
        }

        public async UniTask Join(string roomId, string playerName)
        {
            var packet = new PacketModel<JoinRequest>
            {
                PacketId = PacketId.JoinRequest,
                Payload = new JoinRequest
                {
                    RoomId = roomId,
                    PlayerId = playerName
                }
            };

            await server.Send(packet);
        }

        public async UniTask<List<RoomSession>> Search()
        {
            var packet = new PacketModel<SearchRoomRequest>()
            {
                PacketId = PacketId.SearchRoomRequest
            };

            var response = await server.SendAndReceive(packet, PacketId.SearchRoomResponse);

            //Payload取得
            if (!response.TryGetValue("Payload", out var payload))
            {
                return null;
            }

            var roomList = payload["RoomList"].ToObject<List<RoomSession>>();
            return roomList;
        }

        public RoomSession GetRoomSession()
        {
            return CurrentRoomSession;
        }

        public PlayerSession GetPlayerSession()
        {
            return SelfPlayerSession;
        }

        public void SavePlayerSession(PlayerSession player)
        {
            SelfPlayerSession = player;
        }

        public void SaveRoomSession(RoomSession roomSession)
        {
            CurrentRoomSession = roomSession;
        }

        public IEnumerable<RoomSession> GetAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PlayerSession> GetAllPlayerSession()
        {
            throw new NotImplementedException();
        }

        public void CreatePlayerSession(string id = "", string name = "")
        {
            SelfPlayerSession = new PlayerSession(id, name);
        }
    }
}

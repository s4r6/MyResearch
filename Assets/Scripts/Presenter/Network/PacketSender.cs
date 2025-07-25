using Cysharp.Threading.Tasks;
using Domain.Stage.Object;
using Infrastracture.Network;
using Infrastructure.Network;
using NativeWebSocket;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UseCase.Network;

namespace Presenter.Network
{
    public class PacketSender : IPacketSender
    {
        IWebSocketService socket;

        public PacketSender(IWebSocketService socket)
        {
            this.socket = socket;
        }

        public async UniTask<bool> SendInspectData(InspectResultData data)
        {
            var payload_send = new InspectObjectRequest
            {
                PlayerId = data.PlayerId,
                RoomId = data.RoomId,
                ObjectId = data.ObjectId,
                SelectedChoiceLabel = data.SelectedChoiceLabel
            };

            var packet = CreatePacket(payload_send, PacketId.InspectObjectRequest);

            var response = await socket.SendAndReceive(packet, PacketId.InspectObjectResponse);

            if(!response.TryGetValue("Payload", out var payload_receive))
            {
                return false;
            }

            if (!payload_receive["Success"]?.Value<bool>() ?? false)
                return false;

            return true;
        }

        public async UniTask SendActionData(ActionRequestData data)
        {
            var payload_send = new ActionRequest
            {
                PlayerId = data.PlayerId,
                RoomId = data.RoomId,
                ObjectId = data.ObjectId,
                HeldId = data.HeldId,
                Type = data.Type,
                SelectedActionLabel = data.SelectedActionLabel
            };

            var packet = CreatePacket(payload_send, PacketId.ActionRequest);

            await socket.Send(packet);

            /*if(!response.TryGetValue("Payload", out var payload_receive))
            {
                return ActionResultType.Unknown;
            }

            return payload_receive["Result"].ToObject<ActionResultType>();*/
        }

        public async UniTask SendPlayerPosition(PositionSyncData data)
        {
            Debug.Log("PositionëóêM");
            var payload_send = new PositionUpdate
            {
                PlayerId = data.PlayerId,
                RoomId = data.RoomId,
                X = data.Position.x,
                Y = data.Position.y,
                Z = data.Position.z
            };

            var packet = CreatePacket(payload_send, PacketId.PositionUpdate);
            await socket.Send(packet);
        }

        public async UniTask SendStartVoteData(StartVoteData data)
        {
            var payload_send = new StartVoteRequest
            {
                PlayerId = data.PlayerId,
                RoomId = data.RoomId
            };

            var packet = CreatePacket(payload_send, PacketId.StartVoteRequest);
            await socket.Send(packet);
        }

        public async UniTask SendVoteChoiceData(VoteChoiceData data)
        {
            var payload_send = new VoteChoiceRequest
            {
                PlayerId = data.PlayerId,
                RoomId = data.RoomId,
                Choice = data.Choice
            };

            var packet = CreatePacket(payload_send, PacketId.VoteChoiceRequest);
            await socket.Send(packet);
        }

        PacketModel<T> CreatePacket<T>(T payload, PacketId id)
        {
            var packet = new PacketModel<T>
            {
                PacketId = id,
                Payload = payload
            };

            return packet;
        }


    }
}
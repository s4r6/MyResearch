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

        public async UniTask<ActionResultType> SendActionData(ActionResultData data)
        {
            var payload_send = new ActionRequest
            {
                PlayerId = data.PlayerId,
                RoomId = data.RoomId,
                ObjectId = data.ObjectId,
                SelectedActionLabel = data.SelectedActionLabel
            };

            var packet = CreatePacket(payload_send, PacketId.ActionRequest);

            var response = await socket.SendAndReceive(packet, PacketId.ActionResponse);

            if(!response.TryGetValue("Payload", out var payload_receive))
            {
                return ActionResultType.Unknown;
            }

            return payload_receive["Result"].ToObject<ActionResultType>();
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
using Domain.Stage.Object;
using System.Collections.Generic;

namespace Infrastructure.Network
{
    public enum PacketId
    {
        None = 0,
        JoinRequest,
        JoinResponse,
        CreateRoomRequest,
        CreateRoomResponse,
        SyncObjectDataRequest,
        SyncObjectDataResponse,
        ActionRequest,
        ActionResponse,
        StateUpdate,
        Error,
    }

    public interface IPacketModel
    {
        PacketId PacketId { get; }
        object Payload {  get; }
    }
    public struct PacketModel<T> : IPacketModel
    {
        public PacketId PacketId { get; set; }
        public T Payload { get; set; }
        object IPacketModel.Payload => Payload!;
    }

    public class CreateRoomRequest
    {
        public string RoomId { get; set; }
        public int StageId { get; set; }
        public string PlayerId { get; set; }
    }

    public class CreateRoomResponse
    {
        public bool Success { get; set; }
        public string RoomId { set; get; }
        public string ConnectionId { get; set; }
        public List<SyncObjectPacket> SyncData { get; set; }
    }

    public class JoinRequest
    {
        public string RoomId { get; set; }
        public int StageId { get; set; }
        public string PlayerId { get; set; }

    }

    public class JoinResponse
    {
        public bool Success { get; set; }
        public string PlayerId { get; set; }
        public string RoomId { get; set; }
        public string ConnectionId { get; set; }
        public List<SyncObjectPacket> SyncData { get; set; }
    }

    public class SyncObjectPacket
    {
        public string objectId { get; set; }
        public List<IGameComponentDTO> Components { get; set; } = new();

    }

    public interface IGameComponentDTO
    {
        string Type { get; }
    }

    public class ActionComponentData : IGameComponentDTO
    {
        public string Type { get; set; }
        public string NeedAttribute { get; set; }
    }

    public class CarriableComponentData : IGameComponentDTO
    {
        public string Type { get; set; }
    }

    public class ChoicableComponentData : IGameComponentDTO
    {
        public string Type { get; set; }
        public List<Choice> Choices { get; set; }
        public Choice SelectedChoice { get; set; }
    }

    public class InspectableComponentData : IGameComponentDTO
    {
        public string Type { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool IsActioned { get; set; }
    }

    public class DoorComponentData : IGameComponentDTO
    {
        public string Type { get; set; }
        public bool IsOpen { get; set; }
        public bool IsLock { get; set; }
    }
}

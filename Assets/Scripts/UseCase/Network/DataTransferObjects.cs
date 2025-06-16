using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Stage.Object;

namespace UseCase.Network.DTO
{
    public class CreateRoomInputData
    {
        public string RoomId { get; set; }
        public string PlayerName {  get; set; }
        public int StageId {  get; set; }
    }

    public class CreateRoomOutputData
    {
        public bool Success { get; set; }
        public string RoomId { get; set; }
        public string ConnectionId { get; set; }
        public Dictionary<string, ObjectEntity> ObjectDatas { get; set; }
    }

    public class JoinRoomInputData
    {
        public string PlayerId { get; set; }
        public string RoomId { get; set; }
    }

    public class JoinRoomOutputData
    {
        public bool Success { get; set; }
        public string RoomId { get; set; }
        public string ConnectionId { get; set; }
    }
}

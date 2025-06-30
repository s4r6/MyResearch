using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Domain.Stage.Object;

namespace UseCase.Network
{
    public struct InspectResultData
    {
        public string PlayerId { get; set; }
        public string RoomId { get; set; }
        public string ObjectId { get; set; }
        public string SelectedChoiceLabel { get; set; }
    }

    public struct ActionResultData
    {
        public string PlayerId { get; set; }
        public string RoomId { get; set;}
        public string ObjectId { get; set;}
        public string SelectedActionLabel {  get; set; }
    }

    public enum ActionResultType
    {
        Success,
        ShortageActionPoint,
        Unknown
    }

    public struct ApplyActionResult
    {
        public ActionResultType result { get; set; }
    }

    public interface IPacketSender
    {
        public UniTask<bool> SendInspectData(InspectResultData data);
        public UniTask<ActionResultType> SendActionData(ActionResultData data);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Domain.Action;
using Domain.Stage.Object;
using UnityEngine;

namespace UseCase.Network
{
    public struct InspectResultData
    {
        public string PlayerId { get; set; }
        public string RoomId { get; set; }
        public string ObjectId { get; set; }
        public string SelectedChoiceLabel { get; set; }
        public TimeSpan ElapsedTime { get; set; }
    }

    public struct ActionRequestData
    {
        public string PlayerId { get; set; }
        public string RoomId { get; set;}
        public string ObjectId { get; set;}
        public string HeldId {  get; set; }
        public TargetType Type { get; set; }
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

    public struct PositionSyncData
    {
        public string PlayerId { get; set; }
        public string RoomId { get; set; }
        public Vector3 Position { get; set; }
    }

    public struct StartVoteData
    {
        public string PlayerId { get; set; }
        public string RoomId { get; set; }
    }

    public struct VoteChoiceData
    {
        public string RoomId { get; set;}
        public string PlayerId { get; set;}
        public VoteChoice Choice { get; set; }
    }

    public interface IPacketSender
    {
        public UniTask<bool> SendInspectData(InspectResultData data);
        public UniTask SendActionData(ActionRequestData data);
        public UniTask SendPlayerPosition(PositionSyncData data);
        public UniTask SendStartVoteData(StartVoteData data);
        public UniTask SendVoteChoiceData(VoteChoiceData data);
    }
}

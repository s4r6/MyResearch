using System;
using System.Collections.Generic;
using Domain.Action;
using Domain.Network;
using Domain.Stage;
using Domain.Stage.Object;
using Infrastructure.Network;
using UniRx;
using UnityEngine;
using UseCase.Network;

public struct CreateRoomResult
{
    public bool Success {  get; set; }
    public string RoomId {  get; set; }
    public string ConnectionId {  get; set; }
    public string Name { get; set; }
    public int StageId {  get; set; }
    public List<ObjectEntity> ObjectData { get; set; }
    public int MaxRiskAmount {  get; set; }
    public int MaxActionPointAmount {  get; set; }
}

public struct JoinRoomResult
{
    public bool Success { get; set; }
    public string RoomId { get; set; }
    public string ConnectionId { get; set; }
    public string Name { get; set; }
    public int StageId { get; set; }
    public List<PlayerSession> Players { get; set; }
    public List<ObjectEntity> ObjectData { get; set; }
    public int MaxRiskAmount { get; set; }
    public int MaxActionPointAmount { get; set; }
}

public struct JoinPlayerData
{
    public string JoinedPlayerConncetionId {  get; set; }
    public string JoinedPlayerName {  get; set; }
}

public struct PositionData
{
    public string PlayerId { get; set; }
    public Position Pos {  get; set; }
}

public struct VoteEndData
{
    public VoteResult Result { get; set; }
}

public struct VoteNotifierData
{
    public int Yes;
    public int No;
    public int Total;

    public VoteNotifierData(int yes, int no, int total)
    {
        Yes = yes; 
        No = no; 
        Total = total;
    }
}

public struct DisconnectData
{
    public string DisconnectedId {  get; set; }
}
public struct ActionResultData
{
    public ActionResultType Result { get; set; }
    public TargetType Target { get; set; }
    public string ActionId { get; set; }
    public string TargetId { get; set; }   //見ているオブジェクトのId
    public string HeldId { get; set; }     //手に持っているオブジェクトのId
    public ObjectEntity ObjectData { get; set; }
    public int currentRiskAmount { get; set; }
    public int currentActionPointAmount { get; set; }
    public List<RiskAssessmentHistory> histories { get; set; }
}

public interface IPacketReceiver
{
    public Subject<CreateRoomResult> OnReceiveCreateResponse { get; }
    public Subject<JoinRoomResult> OnReceiveJoinResponse { get; }
    public Subject<JoinPlayerData> OnReceiveJoinPlayerData {  get; }
    public Subject<PositionData> OnMove { get; }
    public Subject<ActionResultData> OnReceiveActionResultData { get; }
    public Subject<VoteNotifierData> OnReceiveVoteNotifierData { get; }
    public Subject<VoteEndData> OnReceiveVoteResult { get; }
    public Subject<DisconnectData> OnReceiveDisconnectData { get; }

}

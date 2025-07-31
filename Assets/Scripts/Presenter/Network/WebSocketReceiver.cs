using System;
using System.Collections.Generic;
using Domain.Network;
using Domain.Stage.Object;
using Infrastracture.Network;
using Infrastructure.Network;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UniRx;
using UnityEngine;
using UseCase.Network;

namespace Presenter.Network
{
    public class WebSocketReceiver : IPacketReceiver, IDisposable
    {
        IWebSocketService server;

        public Subject<CreateRoomResult> OnReceiveCreateResponse { get; } = new();
        public Subject<JoinRoomResult> OnReceiveJoinResponse { get; } = new();
        public Subject<JoinPlayerData> OnReceiveJoinPlayerData { get; } = new();
        public Subject<PositionData> OnMove { get; } = new();
        public Subject<VoteEndData> OnReceiveVoteResult {  get; } = new();
        public Subject<VoteNotifierData> OnReceiveVoteNotifierData { get; } = new();
        public Subject<DisconnectData> OnReceiveDisconnectData { get; } = new();
        public Subject<ActionResultData> OnReceiveActionResultData { get; } = new(); 

        CompositeDisposable _disposables = new();

        public WebSocketReceiver(IWebSocketService server)
        {
            this.server = server;

            Bind();
        }

        public void OnReceivePositionPacket(PositionUpdate data)
        {
            var dto = new PositionData
            {
                PlayerId = data.PlayerId,
                Pos = new Position(data.X, data.Y, data.Z),
            };

            OnMove.OnNext(dto);
        }

        public void OnReceiveNotifierPacket(JoinPlayerNotifier data) 
        {
            var dto = new JoinPlayerData
            {
                JoinedPlayerConncetionId = data.JoinedPlayerId,
                JoinedPlayerName = data.JoinedPlayerName
            };

            OnReceiveJoinPlayerData.OnNext(dto);
        }

        public void OnReceiveCreateResponsePacket(CreateRoomResponse data, List<ObjectEntity> entities)
        {
            var dto = new CreateRoomResult
            {
                Success = data.Success,
                RoomId = data.RoomId,
                RoomName = data.RoomName,
                PlayerId = data.PlayerId,
                PlayerName = data.PlayerName,
                StageId = data.StageId,
                ObjectData = entities,
                MaxRiskAmount = data.MaxRiskAmount,
                MaxActionPointAmount = data.MaxActionPointAmount,
            };

            OnReceiveCreateResponse.OnNext(dto);
        }

        public void OnReceiveJoinResponsePacket(JoinResponse data,  List<ObjectEntity> entities)
        {
            var dto = new JoinRoomResult
            {
                Success = data.Success,
                RoomId = data.RoomId,
                RoomName = data.RoomName,
                PlayerId = data.PlayerId,
                PlayerName = data.PlayerName,
                StageId = data.StageId,
                Players = data.Players,
                ObjectData = entities,
                MaxRiskAmount = data.MaxRiskAmount,
                MaxActionPointAmount = data.MaxActionPointAmount
            };

            OnReceiveJoinResponse.OnNext(dto);
        }

        public void OnReceiveActionResponsePacket(ActionResponse data, ObjectEntity entity)
        {
            var dto = new ActionResultData
            {
                Result = data.Result,
                Target = data.Target,
                ActionId = data.ActionId,
                TargetId = data.TargetId,
                HeldId = data.HeldId,
                ObjectData = entity,
                currentRiskAmount = data.currentRiskAmount,
                currentActionPointAmount = data.currentActionPointAmount,
                histories = data.histories
            };

            OnReceiveActionResultData.OnNext(dto);
        }

        public void OnReceiveVoteEndNotifier(VoteEndNotifier data)
        {
            var dto = new VoteEndData
            {
                Result = data.Result,
            };

            OnReceiveVoteResult.OnNext(dto);
        }

        public void OnReceiveVoteNotifier(VoteNotifier data)
        {
            var dto = new VoteNotifierData
            {
                Yes = data.Yes,
                No = data.No,
                Total = data.Total,
            };

            OnReceiveVoteNotifierData.OnNext(dto);  
        }

        public void OnReceiveDisconnectNotifier(DisconnectNotifier data)
        {
            var dto = new DisconnectData
            {
                DisconnectedId = data.DisconnectedId
            };

            OnReceiveDisconnectData.OnNext(dto);  
        }

        void Bind()
        {
            #region Subscribe

            server.OnMessage
                .Where(tuple => tuple.Item1 == PacketId.PositionUpdate)
                .Subscribe(tuple =>
                {
                    var payload = tuple.Item2["Payload"];
                    var position = payload.ToObject<PositionUpdate>();
                    OnReceivePositionPacket (position);
                }).AddTo(_disposables);

            server.OnMessage
                .Where(tuple => tuple.Item1 == PacketId.JoinPlayerNotifier)
                .Subscribe(tuple =>
                {
                    var payload = tuple.Item2["Payload"];
                    var data = payload.ToObject<JoinPlayerNotifier>();
                    OnReceiveNotifierPacket(data);
                }).AddTo (_disposables);

            server.OnMessage
                .Where(tuple => tuple.Item1 == PacketId.CreateRoomResponse)
                .Subscribe(tuple =>
                {
                    var payload = tuple.Item2["Payload"] as JObject;
                    if (payload == null)
                    {
                        Debug.LogError("Payload is not a valid JObject.");
                        return;
                    }

                    // SyncDataを分離
                    var syncDataToken = payload["SyncData"];
                    payload.Remove("SyncData");

                    // 本体をデシリアライズ
                    var data = payload.ToObject<CreateRoomResponse>();

                    // SyncDataからエンティティ生成
                    var entities = ObjectSerializer.ToEntities(syncDataToken);

                    // ハンドリング
                    OnReceiveCreateResponsePacket(data, entities);
                }).AddTo(_disposables);

            server.OnMessage
                .Where(tuple => tuple.Item1 == PacketId.JoinResponse)
                .Subscribe(tuple => 
                {
                    var payload = tuple.Item2["Payload"] as JObject;
                    if (payload == null)
                    {
                        Debug.LogError("Payload is not a valid JObject.");
                        return;
                    }

                    // SyncDataを分離
                    var syncDataToken = payload["SyncData"];
                    List<ObjectEntity> entities = new();
                    if(syncDataToken != null && syncDataToken.Type != JTokenType.Null)
                    {
                        payload.Remove("SyncData");
                        // SyncDataからエンティティ生成
                        entities = ObjectSerializer.ToEntities(syncDataToken);
                    }

                    // 本体をデシリアライズ
                    var data = payload.ToObject<JoinResponse>();

                    // ハンドリング
                    OnReceiveJoinResponsePacket(data, entities);
                }).AddTo(_disposables);

            server.OnMessage
                .Where(tuple => tuple.Item1 == PacketId.ActionResponse)
                .Subscribe(tuple =>
                {
                    var payload = tuple.Item2["Payload"] as JObject;
                    if (payload == null)
                    {
                        Debug.LogError("Payload is not a valid JObject.");
                        return;
                    }

                    // SyncDataを分離
                    var syncDataToken = payload["SyncData"];
                    ObjectEntity entity = null;
                    if (syncDataToken != null && syncDataToken.Type != JTokenType.Null)
                    {
                        payload.Remove("SyncData");
                        // SyncDataからエンティティ生成
                        entity = ObjectSerializer.ToEntity(syncDataToken);
                    }

                    // 本体をデシリアライズ
                    var data = payload.ToObject<ActionResponse>();

                    // ハンドリング
                    OnReceiveActionResponsePacket(data, entity);
                }).AddTo(_disposables);

            server.OnMessage
                .Where(tuple => tuple.Item1 == PacketId.VoteNotifier)
                .Subscribe(tuple =>
                {
                    var payload = tuple.Item2["Payload"];
                    var notifier = payload.ToObject<VoteNotifier>();
                    OnReceiveVoteNotifier(notifier);
                }).AddTo(_disposables);

            server.OnMessage
                .Where(tuple => tuple.Item1 == PacketId.VoteEndNotifier)
                .Subscribe(tuple =>
                {
                    var payload = tuple.Item2["Payload"];
                    var notifier = payload.ToObject<VoteEndNotifier>();
                    OnReceiveVoteEndNotifier(notifier);
                }).AddTo(_disposables);

            server.OnMessage
                .Where(tuple => tuple.Item1 == PacketId.DisconnectNotifier)
                .Subscribe(tuple =>
                {
                    var payload = tuple.Item2["Payload"];
                    var notifier = payload.ToObject<DisconnectNotifier>();
                    OnReceiveDisconnectNotifier(notifier);
                }).AddTo(_disposables);
            #endregion
        }

        public void Dispose()
        {
            _disposables.Dispose(); 
        }

    }
}
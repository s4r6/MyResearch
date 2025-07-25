using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Infrastructure.Network;
using NativeWebSocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UniRx;
using UnityEngine;

namespace Infrastracture.Network
{
    public class NativeWebSocketService : MonoBehaviour, IWebSocketService
    {
        bool IsRemote = true;
        private WebSocket _socket;
        
        Subject<(PacketId, JObject)> OnReceiveMessage = new Subject<(PacketId, JObject)>();
        public Subject<(PacketId, JObject)> OnMessage => OnReceiveMessage;
        readonly Dictionary<PacketId, TaskCompletionSource<JObject>> _responseWaiters = new();

        public WebSocketState ConnectionState => _socket.State;
        public bool IsConnected => _socket.State == WebSocketState.Open;

        void Awake()
        {
            DontDestroyOnLoad(this);
        }

        async void Start()
        {

#if UNITY_WEBGL && !UNITY_EDITOR
            if(!IsRemote)
                _socket = new WebSocket("wss://192.168.174.114:443/ws"); // ← 本番用
            else
                _socket = new WebSocket("wss://150.59.175.170:443/ws"); 
#else
            //_socket = new WebSocket("ws://localhost:5001/ws"); // ← ローカルテスト用
            _socket = new WebSocket("wss://150.59.175.170/ws");
#endif

            _socket.OnOpen += () =>
            {
                Debug.Log("Connection opened");
                Debug.Log(IsConnected);
            };

            _socket.OnError += (e) =>
            {
                Debug.LogError($"WebSocket Error: {e}");
            };

            _socket.OnClose += (e) =>
            {
                Debug.Log("Connection closed");
            };

            _socket.OnMessage += (bytes) =>
            {
                // UTF-8バイト列をstringに変換
                string json = Encoding.UTF8.GetString(bytes);
                Debug.Log("Received JSON: " + json);

                // デシリアライズ（JsonUtilityを使用）
                var jObject = JObject.Parse(json);
                var packetId = jObject.TryGetValue("PacketId", out var token) && Enum.TryParse(token.ToString(), out PacketId id)
                        ? id : PacketId.None;

                if(_responseWaiters.TryGetValue(packetId, out var tcs))
                {
                    _responseWaiters.Remove(packetId);
                    tcs.SetResult(jObject);
                }
                OnReceiveMessage.OnNext((packetId, jObject));
            };

            
        }

        public async UniTask Connect(string url = "ws://localhost:5001/ws")
        {
#if UNITY_EDITOR
            await _socket.Connect();
#else
            await _socket.Connect();
#endif
        }

#if !UNITY_WEBGL || UNITY_EDITOR
        private void Update() => _socket.DispatchMessageQueue();
#endif

        public async UniTask Send<T>(PacketModel<T> data)
        {
            var json = JsonConvert.SerializeObject(data);

            await _socket.SendText(json);
        }

        public async UniTask<JObject> SendAndReceive<TRequest>(PacketModel<TRequest> data, PacketId responseId)
        {
            var tcs = new TaskCompletionSource<JObject>();
            _responseWaiters[responseId] = tcs;

            var json = JsonConvert.SerializeObject(data);
            await _socket.SendText(json);
            var responseJson = await tcs.Task;

            return responseJson;
        }
    }
}

using UnityEngine;
using NativeWebSocket;
using Cysharp.Threading.Tasks;
using Infrastructure.Network;
using Newtonsoft.Json;

public class WebSocketTest : MonoBehaviour
{
    private WebSocket webSocket;

    private async void Start()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        webSocket = new WebSocket("wss://192.168.174.114:443/ws"); // ← 本番用
#else
        webSocket = new WebSocket("ws://localhost:5001/ws"); // ← ローカルテスト用
#endif

        webSocket.OnOpen += () =>
        {
            Debug.Log("Connection opened!");
            SendJoinRequest("TestPlayer").Forget();
        };

        webSocket.OnError += (e) =>
        {
            Debug.LogError($"WebSocket Error: {e}");
        };

        webSocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed.");
        };

        webSocket.OnMessage += (bytes) =>
        {
            string message = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log($"Received message: {message}");

            var packetId = JsonConvert.DeserializeObject<PacketModel<object>>(message).PacketId;
            if (packetId == PacketId.JoinResponse)
            {
                var response = JsonConvert.DeserializeObject<PacketModel<JoinResponse>>(message);
                Debug.Log($"[JoinResponse] Success: {response.Payload.Success}, PlayerId: {response.Payload.PlayerId}");
            }
        };

        await webSocket.Connect();
    }

    private async UniTaskVoid SendJoinRequest(string playerId)
    {
        var joinRequest = new PacketModel<JoinRequest>
        {
            PacketId = PacketId.JoinRequest
        };

        string json = JsonConvert.SerializeObject(joinRequest);
        Debug.Log($"Send: {json}");
        await webSocket.SendText(json);
    }

    private void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        webSocket?.DispatchMessageQueue(); // WebGL以外ではこれが必要
#endif
    }

    private async void OnApplicationQuit()
    {
        await webSocket.Close();
    }
}

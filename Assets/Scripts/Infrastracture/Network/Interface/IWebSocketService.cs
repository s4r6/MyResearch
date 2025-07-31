using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Infrastructure.Network;
using Newtonsoft.Json.Linq;
using UniRx;

namespace Infrastracture.Network
{
    public interface IWebSocketService
    {
        bool IsConnected { get; }
        UniTask Connect(string url = "ws://localhost:5001/ws");
        Subject<(PacketId, JObject)> OnMessage { get; }
        UniTask Send<T>(PacketModel<T> data);
        UniTask<JObject> SendAndReceive<TRequest>(PacketModel<TRequest> data, PacketId responseId);
        UniTask Close();
    }
}

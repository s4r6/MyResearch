using Presenter.Network;
using UnityEngine;

namespace Infrastracture.Network
{
    public class ReceiverHolder : MonoBehaviour
    {
        public WebSocketReceiver receiver { get; private set; }

        public void SetReceiver(WebSocketReceiver receiver)
        {
            this.receiver = receiver;
        }

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }
    }
}
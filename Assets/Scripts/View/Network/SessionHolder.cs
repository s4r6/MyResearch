using Domain.Network;
using UnityEngine;

namespace Infrastructure.Network
{
    public class SessionHolder : MonoBehaviour
    {
        public RoomSession room { get; private set; }

        public void SetSession(RoomSession room)
        {
            this.room = room;
        }

        void Awake()
        {
            DontDestroyOnLoad(this);
        }
    }
}
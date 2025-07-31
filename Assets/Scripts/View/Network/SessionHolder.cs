using Domain.Network;
using Infrastracture.Repository;
using UnityEngine;

namespace Infrastructure.Network
{
    public class SessionHolder : MonoBehaviour
    {
        public RoomRepository room { get; private set; }

        public void SetSession(RoomRepository room)
        {
            this.room = room;
        }

        void Awake()
        {
            DontDestroyOnLoad(this);
        }
    }
}
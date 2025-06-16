using UnityEngine;

namespace Domain.Network
{
    public class PlayerSession
    {
        public string Id { get; }
        public string Name { get; }
        public PlayerSession(string id, string name) { Id = id; Name = name; }
    }
}
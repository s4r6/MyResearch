using UnityEngine;

namespace Domain.Network
{
    public class PlayerSession
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public PlayerSession(string id, string name) { Id = id; Name = name; }
    
        public void SetName(string name) { Name = name; }

        public void SetId(string id) { Id = id; }
    }
}
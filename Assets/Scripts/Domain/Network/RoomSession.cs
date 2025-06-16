using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Domain.Network
{
    public class RoomSession
    {
        public string Id { get; }
        public List<PlayerSession> Players;

        public RoomSession(string id, List<PlayerSession> players) 
        { 
            Id = id; 
            Players = players;
        }
        
    }
}
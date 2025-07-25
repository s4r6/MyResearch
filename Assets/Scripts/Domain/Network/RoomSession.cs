using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Domain.Network
{
    public class RoomSession
    {
        public string Id { get; }
        public List<PlayerSession> Players;
        public int StageId {  get; }

        public RoomSession(string id, List<PlayerSession> players, int stageId) 
        { 
            Id = id; 
            Players = players;
            StageId = stageId;
        }

        public void AddPlayer(PlayerSession player) 
        { 
            Players.Add(player);
        }

        public void RemovePlayer(PlayerSession player) 
        { 
            Players.Remove(player);
        }
        
    }
}
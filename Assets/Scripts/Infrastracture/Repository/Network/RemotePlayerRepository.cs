using System.Collections.Generic;
using System.Linq;
using Domain.Network;
using UnityEngine;
using UseCase.Network;

namespace Infrastracture.Repository
{
    public class RemotePlayerRepository : IRemotePlayerRepository
    {
        Dictionary<string, RemotePlayerEntity> entities = new();

        public void Add(string id, RemotePlayerEntity entity)
        {
            entities.Add(id, entity);
        }

        public RemotePlayerEntity Get(string id)
        {
            if(!entities.TryGetValue(id, out RemotePlayerEntity entity))
            {
                return null;
            }

            return entity;
        }

        public IReadOnlyList<RemotePlayerEntity> GetAll()
        {
            return entities.Values.ToList();
        }

        public void Remove(string id)
        {
            entities.Remove(id);
        }
    }
}
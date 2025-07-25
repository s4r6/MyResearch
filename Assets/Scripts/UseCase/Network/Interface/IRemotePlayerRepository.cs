using System.Collections.Generic;
using Domain.Network;
using UnityEngine;

namespace UseCase.Network
{
    public interface IRemotePlayerRepository
    {
        RemotePlayerEntity Get(string id);
        void Add(string id, RemotePlayerEntity entity);
        IReadOnlyList<RemotePlayerEntity> GetAll();
        void Remove(string id);
    }
}
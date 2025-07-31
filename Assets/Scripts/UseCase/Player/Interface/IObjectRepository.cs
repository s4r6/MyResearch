using System.Collections.Generic;
using Domain.Stage;
using Domain.Stage.Object;
using UnityEngine;

namespace UseCase.Player
{
    public interface IObjectRepository
    {
        void Save(ObjectEntity entity);
        ObjectEntity GetById(string objectId);
        IReadOnlyList<ObjectEntity> GetAll();
    }
}
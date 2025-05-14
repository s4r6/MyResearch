using Domain.Stage;
using Domain.Stage.Object;
using UnityEngine;

public interface IObjectRepository
{
    StageEntity LoadStageEntity();
    ObjectEntity LoadObjectEntity(string id);
    CarriableObject LoadCarriable(string id);
}

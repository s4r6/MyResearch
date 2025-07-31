using Domain.Stage;
using UnityEngine;

public interface IStageRepository
{
    void Save(StageEntity stage);
    StageEntity GetCurrentStageEntity();
    StageEntity CreateStage(int stageNumber);
}

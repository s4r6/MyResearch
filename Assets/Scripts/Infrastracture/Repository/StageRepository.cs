using System.Collections.Generic;
using System.Linq;
using Domain.Component;
using Domain.Stage.Object;
using UnityEngine;

namespace Infrastructure.Repository
{
    //ステージのデータを保持しておく
    public class StageRepository
    {
        Dictionary<int, int> MaxRiskMap = new();
        Dictionary<int, int> MaxActionPointMap = new();

        public StageRepository(ObjectRepository repository)
        {
            var entities = repository.GetAllEntity();
            MaxRiskMap.Add(1, CalcMaxRiskAmount(entities));
            MaxActionPointMap.Add(1, CalcMaxActionPoint(entities));
        }

        public int GetRiskAmountByStageNumber(int stageNumber)
        {
            return MaxRiskMap[stageNumber];
        }

        public int GetActionPointAmountByStageNumber(int stageNumber) 
        {
            return MaxActionPointMap[stageNumber];
        }

        int CalcMaxRiskAmount(List<ObjectEntity> entities)
        {
            Debug.Log("読み込んだEntityの数:" + entities.Count);
            int totalMaxRisk = entities
                                   .Where(e => e.HasComponent<InspectableComponent>())
                                   .Select(e =>
                                   {
                                       var inspectable = e.GetComponent<InspectableComponent>();
                                       return inspectable.Choices
                                                         .Where(c => c.OverrideActions.Any())
                                                         .Select(c => c.OverrideActions.Min(a => a.riskChange))
                                                         .DefaultIfEmpty(0)
                                                         .Min();
                                   })
                                   .Sum(riskChange => -riskChange);

            Debug.Log("totalMaxRisk:" + totalMaxRisk);
            return totalMaxRisk;
        }

        int CalcMaxActionPoint(List<ObjectEntity> entities) 
        {
            int totalMaxActionPoint = entities
                                            .Where(e => e.HasComponent<InspectableComponent>())
                                            .Select(e =>
                                            {
                                                var inspectable = e.GetComponent<InspectableComponent>();
                                                return inspectable.Choices
                                                                  .Where (c => c.OverrideActions.Any())
                                                                  .Select(c => c.OverrideActions.Max(a => a.actionPointCost))
                                                                  .DefaultIfEmpty(0)
                                                                  .Max();
                                            })
                                            .Sum(actionPointCost => actionPointCost);

            return totalMaxActionPoint;
        }
    }
}
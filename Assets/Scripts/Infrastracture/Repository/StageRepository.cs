using System.Collections.Generic;
using System.Linq;
using Domain.Component;
using Domain.Stage.Object;
using UnityEngine;
using UseCase.Player;

namespace Infrastructure.Repository
{
    //ステージのデータを保持しておく
    public class StageRepository
    {
        Dictionary<int, int> MaxRiskMap = new();
        Dictionary<int, int> MaxActionPointMap = new();

        public StageRepository(IObjectRepository repository)
        {
            var entities = repository.GetAll();
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

        int CalcMaxRiskAmount(IReadOnlyList<ObjectEntity> entities)
        {
            int totalMaxRisk = entities
                                   .Where(e => e.HasComponent<ChoicableComponent>())
                                   .Select(e =>
                                   {
                                       var choicable = e.GetComponent<ChoicableComponent>();
                                       return choicable.Choices
                                                         .Where(c => c.OverrideActions.Any())
                                                         .Select(c => c.OverrideActions.Min(a => a.riskChange))
                                                         .DefaultIfEmpty(0)
                                                         .Min();
                                   })
                                   .Sum(riskChange => -riskChange);

            return totalMaxRisk;
        }

        int CalcMaxActionPoint(IReadOnlyList<ObjectEntity> entities)
        {
            int totalActionPoint = entities
                .Where(e => e.HasComponent<ChoicableComponent>())
                .Select(e =>
                {
                    var choicable = e.GetComponent<ChoicableComponent>();

                    // すべてのChoice内のActionを1つのシーケンスに平坦化
                    var minRiskAction = choicable.Choices
                        .Where(c => c.OverrideActions != null && c.OverrideActions.Any())
                        .SelectMany(c => c.OverrideActions)
                        .OrderBy(a => a.riskChange)
                        .FirstOrDefault();

                    // ActionEntityがなければ0、あればそのactionPointCostを返す
                    return minRiskAction?.actionPointCost ?? 0;
                })
                .Sum();

            return totalActionPoint;
        }
    }
}
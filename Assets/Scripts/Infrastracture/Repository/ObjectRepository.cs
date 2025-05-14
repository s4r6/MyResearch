using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Domain.Stage;
using Domain.Stage.Object;
using UnityEngine;

namespace Infrastructure.Repository
{
    public class ObjectRepository : IObjectRepository
    {
        Dictionary<string, ObjectEntity> ObjectEntities = new();
        Dictionary<string, CarriableObject> CarriableObjects = new();
        List<StageEntity> defaultStages = new();

        public void Initialize()
        {
            var carriablesJson = Resources.Load<TextAsset>("Master/CarriableObjects");
            var effectsJson = Resources.Load<TextAsset>("Master/ActionEffects");
            var choicesJson = Resources.Load<TextAsset>("Master/Choices");
            var InspectableJson = Resources.Load<TextAsset>("Master/InspectableObjects");
            var ObjectEntitiyJson = Resources.Load<TextAsset>("Master/ObjectEntities");

            var carriableSerializer = new JsonListSerializer<CarriableObject>();
            var choiceSerializer = new JsonListSerializer<Choice>();
            var InspectableSerializer = new JsonListSerializer<InspectableObject>();
            var ObjectEnitiySerializer = new JsonListSerializer<ObjectEntity>();

            CarriableObjects = carriableSerializer.Load(carriablesJson.text).ToDictionary(x => x.name);
            //var actionables = actionableSerializer.Load(actionablesJson.text);
            //var choices = choiceSerializer.Load(choicesJson.text);
            var inspectableObjects = InspectableSerializer.Load(InspectableJson.text);
            var objects = ObjectEnitiySerializer.Load(ObjectEntitiyJson.text);

            var castedInspectable = inspectableObjects.Cast<ObjectEntity>().ToList();

            objects.AddRange(castedInspectable);

            ObjectEntities = objects
                                .ToDictionary(x => x.ObjectId);


            int totalMaxActionPointCost = 0;
            int totalMinRiskChange = 0;

            foreach (var obj in inspectableObjects)
            {
                int maxActionPointCost = 0;
                int minRiskChange = 0;
                bool found = false;

                foreach (var choice in obj.choices)
                {
                    if (choice.OverrideActions == null) continue;

                    foreach (var action in choice.OverrideActions)
                    {
                        int apCost = action.actionPointCost;
                        int riskChange = action.riskChange;

                        if (!found)
                        {
                            maxActionPointCost = apCost;
                            minRiskChange = riskChange;
                            found = true;
                        }
                        else
                        {
                            if (apCost > maxActionPointCost)
                                maxActionPointCost = apCost;
                            if (riskChange < minRiskChange)
                                minRiskChange = riskChange;
                        }
                    }
                }

                if (found)
                {
                    totalMaxActionPointCost += maxActionPointCost;
                    totalMinRiskChange += minRiskChange * -1;
                }
            }

            Debug.Log($"[集計結果] 最大行動ポイント: {totalMaxActionPointCost}, 最大リスク減少: {totalMinRiskChange}");

            defaultStages.Add(new StageEntity(totalMinRiskChange, totalMaxActionPointCost, new(), new()));
        }

        public ObjectEntity LoadObjectEntity(string objectId)
        {
            if (ObjectEntities.TryGetValue(objectId, out ObjectEntity obj)) 
            {
                Debug.Log("InspectableObject :" + obj.ObjectId);
                return obj; 
            }
            return null;
        }

        public CarriableObject LoadCarriable(string id)
        {
            if (CarriableObjects.TryGetValue(id, out CarriableObject obj)) { return obj; }
            return null;
        }

        public StageEntity LoadStageEntity()
        {
            return defaultStages[0];
        }
    }
}
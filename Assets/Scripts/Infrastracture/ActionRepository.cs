using Infrastructure.Master;
using UnityEngine;
using Domain.Action;
using System.Collections.Generic;
using Domain.Stage.Object;

namespace Infrastructure.Action
{
    public class ActionRepository
    {
        IActionDataProvider provider;
        Dictionary<string, List<ActionEntity>> CashActionEntities = new();
        public ActionRepository(IActionDataProvider provider)
        {
            this.provider = provider;
        }

        public List<InspectableObject> TryGetActionableObjectsById(string objectId)
        {
            var actionableObjects = provider.GetActionableObjectsByObjectId(objectId);

            return actionableObjects;
        }
        public void TryGetActionableObjects(string objectId)
        {
            
        }
        /// <summary>
        /// objectIdを持つオブジェクトの現在可能なアクションのリスト取得
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public List<ActionEntity> TryGetActionEntities(string objectId)
        {
            if(CashActionEntities.ContainsKey(objectId))
            {
                return CashActionEntities[objectId];
            }

            return provider.GetActionEntities(objectId);
        }

        public string? TryGetActionIdByLabel(string label)
        {
            return provider.GetActionId(label);
        }

        public bool SaveActionEntities(string objectId, List<ActionEntity> actionEntities)
        {
            if(CashActionEntities.ContainsKey(objectId))
            {
                return false;
            }

            CashActionEntities.Add(objectId, actionEntities);
            return true;
        }
    }
}
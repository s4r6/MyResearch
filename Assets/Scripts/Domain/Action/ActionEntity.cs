using UnityEngine;

namespace Domain.Action
{
    public class ActionEntity
    {
        public readonly string id;
        public readonly string label;
        public readonly int riskChange;
        public readonly int actionPointCost;
        public readonly bool isSuccess;
        public readonly string targetObjectId;
        public readonly string executeOnObjectId;
    
        public ActionEntity(string id, string label, int riskChange, int actionPointCost, bool isSuccess, string targetObjectId, string executeOnObjectId)
        {
            this.id = id;
            this.label = label;
            this.riskChange = riskChange;
            this.actionPointCost = actionPointCost;
            this.isSuccess = isSuccess;
            this.targetObjectId = targetObjectId;
            this.executeOnObjectId = executeOnObjectId;
        }

        public override string ToString()
        {
            return $"ActionEntity: {id}, {label}, {riskChange}, {actionPointCost}, {isSuccess}, {targetObjectId}, {executeOnObjectId}";
        }
    }
}
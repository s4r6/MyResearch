using UnityEngine;

namespace Domain.Action
{
    public class ActionEntity
    {
        public string id;
        public int riskChange;
        public int actionPointCost;
        public bool isSuccess;
    
        public ActionEntity(string id, string label, int riskChange, int actionPointCost, bool isSuccess)
        {
            this.id = id;
            this.riskChange = riskChange;
            this.actionPointCost = actionPointCost;
            this.isSuccess = isSuccess;

        }

        public override string ToString()
        {
            return $"ActionEntity: {id}, {riskChange}, {actionPointCost}, {isSuccess}";
        }
    }
}
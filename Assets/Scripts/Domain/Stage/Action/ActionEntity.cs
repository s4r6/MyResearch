using UnityEngine;

namespace Domain.Action
{
    public class ActionEntity
    {
        readonly string id;
        readonly int riskChange;
        readonly int actionPointCost;
        readonly bool isSuccess;
    
        public ActionEntity(string id, int riskChange, int actionPointCost, bool isSuccess)
        {
            this.id = id;
            this.riskChange = riskChange;
            this.actionPointCost = actionPointCost;
            this.isSuccess = isSuccess;
        }
    }
}
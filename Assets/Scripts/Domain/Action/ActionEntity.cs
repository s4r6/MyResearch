using UnityEngine;

namespace Domain.Action
{
    public class ActionEntity
    {
        public string id;
        public int riskChange;
        public int actionPointCost;

        public override string ToString()
        {
            return $"ActionEntity: {id}, {riskChange}, {actionPointCost}";
        }
    }
}
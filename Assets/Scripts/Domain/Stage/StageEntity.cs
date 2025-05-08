using System.Collections.Generic;
using UnityEngine;
using Domain.Stage.Object;
using Domain.Action; // InspectableObjectの参照

namespace Domain.Stage
{
    public class StageEntity
    {
        private readonly int maxRiskAmount;
        private readonly int maxActionPoint;
        private int currentRiskAmount;
        private int currentActionPointAmount;
        private readonly List<InspectableObject> inspectableObjects;

        //フラグが立ったActionのリスト
        private readonly List<ActionEntity> actions;

        //持ち運べるオブジェクトのリスト
        private readonly HashSet<string> carriableObjects;

        public event System.Action OnEndStage;

        public StageEntity(int maxRiskAmount, int maxActionPoint, List<InspectableObject> inspectableObjects, List<ActionEntity> actions, HashSet<string> carriableObjects)
        {
            this.maxRiskAmount = maxRiskAmount;
            this.maxActionPoint = maxActionPoint;
            this.currentRiskAmount = 0;
            this.currentActionPointAmount = maxActionPoint;
            this.inspectableObjects = inspectableObjects;
            this.actions = actions;
            this.carriableObjects = carriableObjects;
        }

        public void CalcRiskAmount(ActionEntity action)
        {
            if (currentRiskAmount > maxRiskAmount)
                currentRiskAmount = maxRiskAmount;
        }

        public void CalcActionPointAmount(ActionEntity action)
        {
            if (currentActionPointAmount < 0)
                currentActionPointAmount = 0;
        }

        public int GetRiskAmount()
        {
            return currentRiskAmount;
        }

        public int GetActionPoint()
        {
            return currentActionPointAmount;
        }

        public void AddAction(ActionEntity action)
        {
            actions.Add(action);
        }

        public void EndStage()
        {
            OnEndStage?.Invoke();
        }
    }
}
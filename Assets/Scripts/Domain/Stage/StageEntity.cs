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

        //----------------------リザルト表示用--------------------------
        //調査してリスクを選択したオブジェクトのリスト
        private readonly List<ObjectEntity> InspectedObjectEntities;
        //実行したActionのリスト
        private readonly List<ActionEntity> actions;

        public event System.Action OnEndStage;

        public StageEntity(int maxRiskAmount, int maxActionPoint, List<ObjectEntity> inspectableObjects, List<ActionEntity> actions)
        {
            this.maxRiskAmount = maxRiskAmount;
            this.maxActionPoint = maxActionPoint;
            this.currentRiskAmount = maxRiskAmount;
            this.currentActionPointAmount = maxActionPoint;
            this.InspectedObjectEntities = inspectableObjects;
            this.actions = actions;
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

        public void OnExecuteAction(ActionEntity action)
        {
            if (action == null) return;

            currentActionPointAmount -= action.actionPointCost;
            currentRiskAmount += action.riskChange;
        }

        void AddAction(ActionEntity action)
        {
            actions.Add(action);
        }

        void AddInspect(ObjectEntity inspectable)
        {
            InspectedObjectEntities.Add(inspectable);
        }

        public void EndStage()
        {
            OnEndStage?.Invoke();
        }
    }
}
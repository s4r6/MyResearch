using System.Collections.Generic;
using UnityEngine;
using Domain.Stage.Object;
using Domain.Action; // InspectableObjectの参照

namespace Domain.Stage
{
    public struct RiskAssessmentHistory
    {
        public readonly string ObjectName { get; }
        public readonly string SelectedRiskLable {  get; }
        public readonly string ExecutedActionLabel {  get; }
        public readonly int RiskChange {  get; }
        public readonly int ActionCost {  get; }

        public RiskAssessmentHistory(string name, string riskLabel, string actionLabel, int riskChange, int actionCost)
        {
            ObjectName = name;
            SelectedRiskLable = riskLabel;
            ExecutedActionLabel = actionLabel;
            RiskChange = riskChange;
            ActionCost = actionCost;
        }
    }

    public class StageEntity
    {
        private readonly int maxRiskAmount;
        private readonly int maxActionPoint;
        private int currentRiskAmount;
        private int currentActionPointAmount;

        //----------------------リザルト表示用--------------------------
        public List<RiskAssessmentHistory> histories = new();

        public event System.Action OnEndStage;

        public StageEntity(int maxRiskAmount, int maxActionPoint)
        {
            this.maxRiskAmount = maxRiskAmount;
            this.maxActionPoint = maxActionPoint;
            this.currentRiskAmount = maxRiskAmount;
            this.currentActionPointAmount = maxActionPoint;
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

        public void OnExecuteAction(RiskAssessmentHistory history)
        {
            currentActionPointAmount -= history.ActionCost;
            currentRiskAmount += history.RiskChange;
            AddHistory(history);
        }

        void AddHistory(RiskAssessmentHistory history)
        {
            histories.Add(history);
        }

        public void EndStage()
        {
            OnEndStage?.Invoke();
        }
    }
}
using System.Collections.Generic;
using UnityEngine;
using Domain.Stage.Object;
using Domain.Action;
using UseCase.Player; // InspectableObjectの参照

namespace Domain.Stage
{
    public struct RiskAssessmentHistory
    {
        public string ObjectName { get; }
        public string SelectedRiskLable { get; }
        public string ExecutedActionLabel { get; }

        public int RiskChange { get; } // 実行による変化量
        public int CurrentRisk { get; }
        public int MaxRisk { get; }

        public int ActionCost { get; } // 使用したアクションポイント
        public int CurrentActionPoint { get; }
        public int MaxActionPoint { get; }

        public RiskAssessmentHistory(
            string objectName,
            string riskLabel,
            string actionLabel,
            int riskChange,
            int currentRisk,
            int maxRisk,
            int actionCost,
            int currentAP,
            int maxAP)
        {
            ObjectName = objectName;
            SelectedRiskLable = riskLabel;
            ExecutedActionLabel = actionLabel;
            RiskChange = riskChange;
            CurrentRisk = currentRisk;
            MaxRisk = maxRisk;
            ActionCost = actionCost;
            CurrentActionPoint = currentAP;
            MaxActionPoint = maxAP;
        }
    }


    public class StageEntity
    {
        private readonly int maxRiskAmount;
        public readonly int maxActionPoint;
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

        public void OnExecuteAction(ActionHistory history)
        {
            currentActionPointAmount -= history.ActionCost;
            currentRiskAmount += history.RiskChange;

            var riskAssesmentHis = new RiskAssessmentHistory(history.ObjectName, history.SelectedRiskLable, history.ExecutedActionLabel, history.RiskChange, currentRiskAmount, maxRiskAmount, history.ActionCost, currentActionPointAmount, maxActionPoint);
            AddHistory(riskAssesmentHis);
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
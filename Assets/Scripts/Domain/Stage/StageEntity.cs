using System.Collections.Generic;
using UnityEngine;
using Domain.Stage.Object;
using Domain.Action;
using UseCase.Player; // InspectableObjectの参照

namespace Domain.Stage
{
    public struct RiskAssessmentHistory
    {
        public string ObjectName { get; set; }
        public string SelectedRiskLabel { get; set; }
        public string ExecutedActionLabel { get; set; }

        public int RiskChange { get; set; } // 実行による変化量
        public int CurrentRisk { get; set; }
        public int MaxRisk { get; set; }

        public int ActionCost { get; set; }// 使用したアクションポイント
        public int CurrentActionPoint { get; set; }
        public int MaxActionPoint { get; set; }

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
            SelectedRiskLabel = riskLabel;
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

        public void Update(int currentRiskAmount, int currentActionPointAmount, List<RiskAssessmentHistory> histories)
        {
            this.currentRiskAmount = currentRiskAmount;
            this.currentActionPointAmount = currentActionPointAmount;
            this.histories = histories;
            foreach(var history in this.histories)
            {
                Debug.Log(history.SelectedRiskLabel);
            }
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
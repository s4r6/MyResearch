using System.Collections.Generic;
using UnityEngine;
using Domain.Stage.Object;
using Domain.Action;
using UseCase.Player;
using System;
using UniRx;
using Unity.Collections;
using System.Linq;
using Domain.Component;

namespace Domain.Stage
{
    public struct SurmmaryDTO
    {
        public TimeSpan time;
        public int FindRiskNum;                     //発見した潜在リスクの数
        public int MaxRiskNum;                      //潜在リスクの最大数
        public int ExecuteCorrectActionNum;         //実行したリスクが減少する行動の数
        public int MaxCorrectActionNum;             //リスクが減少する行動の最大数
        public int CurrentRisk;
        public int MaxRisk;
        public int CurrentActionPoint;
        public int MaxActionPoint;

        public List<SurmmaryDetailDTO> Actions;     //実行した対応策
    }

    public struct SurmmaryDetailDTO
    {
        public string DisplayName;  //オブジェクトの表示名
        public string RiskLabel;    //選択したリスク名
        public string ActionLabel;  //実行した対応策名
        public int RiskChange;
        public int ActionCost;
        public string Explanation;  //解説
        public string Description;     //状況説明

        public List<string> RiskLabels;
        //DisplayName, <RiskChange, ActionCost>
        public List<(string label, (int, int))> ActionLabels;
    }

    public interface IStageObjectRepository
    {
        IReadOnlyList<ObjectEntity> GetAll();
    }

    public class StageEntity
    {
        readonly IStageObjectRepository repository;
        private readonly int maxRiskAmount;
        public readonly int maxActionPoint;
        private int currentRiskAmount;
        private int currentActionPointAmount;

        //----------------------リザルト表示用--------------------------
        public SurmmaryDTO surmmary;
        public List<SurmmaryDetailDTO> histories = new();

        public event System.Action OnEndStage;

        public StageEntity(int maxRiskAmount, int maxActionPoint, IStageObjectRepository repository)
        {
            this.repository = repository;
            this.maxRiskAmount = maxRiskAmount;
            this.maxActionPoint = maxActionPoint;
            this.currentRiskAmount = maxRiskAmount;
            this.currentActionPointAmount = maxActionPoint;
        }

        public void Update(int currentRiskAmount, int currentActionPointAmount, List<SurmmaryDetailDTO> histories)
        {
            this.currentRiskAmount = currentRiskAmount;
            this.currentActionPointAmount = currentActionPointAmount;
            this.histories = histories;
        }

        public SurmmaryDTO CreateSurmmary()
        {
            if (repository == null) return new SurmmaryDTO();

            var objects = repository.GetAll();
            var findRiskNum = histories.Count(history => history.RiskChange < 0);
            var maxRiskNum = objects.Count(obj =>
            {
                if (obj.TryGetComponent<ChoicableComponent>(out var choicable))
                {
                    var hasRisk = choicable.Choices.Any(choice => choice.OverrideActions.Any(action => action.riskChange < 0));
                    return hasRisk;
                }
                else
                {
                    return false;
                }
            });
            var executeCorrectActionNum = histories.Count(history => history.RiskChange < 0);
            var maxCorrectActionNum = 0;

            return new SurmmaryDTO
            {
                time = TimeSpan.Zero,
                FindRiskNum = findRiskNum,
                MaxRiskNum = maxRiskNum,
                ExecuteCorrectActionNum = executeCorrectActionNum,
                MaxCorrectActionNum = maxCorrectActionNum,
                CurrentRisk = currentRiskAmount,
                MaxRisk = maxRiskAmount,
                CurrentActionPoint = currentActionPointAmount,
                MaxActionPoint = maxActionPoint,

                Actions = histories
            };
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

            var detailDTO = new SurmmaryDetailDTO()
            {
                DisplayName = history.DisplayName,
                Explanation = history.Explanation,
                RiskLabel = history.SelectedRiskLable,
                ActionLabel = history.ExecutedActionLabel,
                RiskChange = history.RiskChange,
                ActionCost = history.ActionCost,
                RiskLabels = history.RiskLabels,
                ActionLabels = history.Actions.Select(action => (action.label, (action.riskChange, action.actionPointCost))).ToList(),
                Description = history.Description,
            };

            AddHistory(detailDTO);
        }

        void AddHistory(SurmmaryDetailDTO history)
        {
            histories.Add(history);
        }

        public void EndStage()
        {
            OnEndStage?.Invoke();
        }
    }
}
using System.Collections.Generic;
using Domain.Stage;
using NUnit.Framework;
using UnityEngine;

namespace View.UI
{
    public class ResultView : MonoBehaviour
    {
        //----------------------VIEW----------------------
        [SerializeField]
        GameObject HistoryViewPrefab;
        [SerializeField]
        Transform ParentTransform;

        List<GameObject> Histories = new();

        void Start()
        {
            gameObject.SetActive(false);    
        }

        public void SetHistory(string name, string risklabel, string actionlabel, string riskReduce, string actionCost)
        {
            var history = Instantiate(HistoryViewPrefab, ParentTransform);
            history.GetComponent<HistroyView>().Initialize(name, risklabel, actionlabel, riskReduce, actionCost);
            Histories.Add(history);
        }

        public void ShowResult()
        {
            gameObject.SetActive(true);
        }

        //-------------------PRESENTER------------------------
        public void ShowResultWindow(List<RiskAssessmentHistory> histories)
        {
            foreach(RiskAssessmentHistory history in histories)
            {
                SetHistory(history.ObjectName, history.SelectedRiskLable, history.ExecutedActionLabel, history.RiskChange.ToString(), history.ActionCost.ToString());
            }

            ShowResult();
        }
    }
}
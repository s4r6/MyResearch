using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
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

        [SerializeField]
        IndicatorView RiskIndicator;
        [SerializeField]
        IndicatorView ActionPointIndicator;

        List<HistroyView> Histories = new();

        void Awake()
        {
            gameObject.SetActive(false);    
        }

        public void SetHistory(HistroyView view, RiskAssessmentHistory history)
        {
            view.SetText(history.ObjectName, history.SelectedRiskLabel, history.ExecutedActionLabel, history.RiskChange.ToString(), history.ActionCost.ToString());
        }

        public void Display()
        {
            Debug.Log("表示");
            this.gameObject.SetActive(true);
        }

        public async UniTask ShowResult(List<RiskAssessmentHistory> histories)
        {
            foreach(RiskAssessmentHistory history in histories)
            {
                //各Historyを作成&テキストの適用
                var hist = Instantiate(HistoryViewPrefab, ParentTransform);
                var view = hist.GetComponent<HistroyView>();
                Histories.Add(view);
                SetHistory(view, history);

                //アニメーションしながら表示
                await view.Display();

                //表示が終わったら
                await UniTask.WhenAll(
                    RiskIndicator.SetValueAsync(history.CurrentRisk, history.MaxRisk),
                    ActionPointIndicator.SetValueAsync(history.CurrentActionPoint, history.MaxActionPoint)
                );
            }
        }

        //-------------------PRESENTER------------------------
        public void ShowResultWindow(List<RiskAssessmentHistory> histories)
        {
            Display();
            ShowResult(histories).Forget();
        }
    }
}
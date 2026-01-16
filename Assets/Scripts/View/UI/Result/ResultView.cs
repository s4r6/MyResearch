using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UniRx;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using View.Player;
using DG.Tweening;
using Domain.Stage;

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

        [SerializeField]
        TMP_Text SelectedRiskNum;
        [SerializeField]
        TMP_Text RiskReducedActionNum;

        List<HistroyView> Histories = new();
        List<GameObject> HistoryButtons = new();

        [SerializeField]
        DetailWindowManager manager;

        bool IsDisplayCompleted = false;
        bool IsSkipRequested = false;
        void Awake()
        {
            gameObject.SetActive(false);    
        }

        public void SetHistory(HistroyView view, SurmmaryDetailDTO detail)
        {
            view.SetText(detail.DisplayName, detail.RiskLabel, detail.ActionLabel, detail.RiskChange.ToString(), detail.ActionCost.ToString());
        }

        public void Display()
        {
            this.gameObject.SetActive(true);

            PlayerView.cursorLocked = false;
            Cursor.lockState = CursorLockMode.None;    // カーソル自由
            Cursor.visible = true;                     // カーソル表示
        }

        public void Skip()
        {
            IsSkipRequested = true;
        }

        public async UniTask ShowResult(SurmmaryDTO surmmary)
        {
            foreach (var action in surmmary.Actions)
            {
                //各Historyを作成&テキストの適用
                var hist = Instantiate(HistoryViewPrefab, ParentTransform);
                var view = hist.GetComponent<HistroyView>();

                HistoryButtons.Add(hist);
                Histories.Add(view);
                SetHistory(view, action);

                hist.GetComponent<Button>().onClick.AddListener(() => ShowDetailPage(view.GetId(), action));

                surmmary.CurrentRisk += action.RiskChange;
                surmmary.CurrentActionPoint -= action.ActionCost;

                if(IsSkipRequested)
                {
                    view.SkipAnimation();

                    RiskIndicator.SetValue(surmmary.CurrentRisk, surmmary.MaxRisk);
                    ActionPointIndicator.SetValue(surmmary.CurrentActionPoint, surmmary.MaxActionPoint);
                }
                else
                {
                    //アニメーションしながら表示
                    await view.Display();

                    //表示が終わったら
                    await UniTask.WhenAll(
                        RiskIndicator.SetValueAsync(surmmary.CurrentRisk, surmmary.MaxRisk),
                        ActionPointIndicator.SetValueAsync(surmmary.CurrentActionPoint, surmmary.MaxActionPoint)
                    );
                }
            }

            IsDisplayCompleted = true;
        }

        public void FocusToElement(string elementId, SurmmaryDetailDTO data)
        {
            foreach (var element in Histories)
            {

                if (element.GetId() != elementId)
                    continue; 

                manager.DisplayDetail(data);

                manager.onClosed
                    .Take(1)
                    .Subscribe(_ => gameObject.SetActive(true));

                gameObject.SetActive(false);
            }
        }

        //-------------------PRESENTER------------------------
        public void ShowResultWindow(SurmmaryDTO surmmary)
        {
            Display();
            SelectedRiskNum.text = $"{surmmary.FindRiskNum} / {surmmary.MaxRiskNum}";
            //RiskReducedActionNum.text = $"{surmmary.ExecuteCorrectActionNum} / {surmmary.MaxCorrectActionNum}";
            ShowResult(surmmary).Forget();
        }

        public void ShowDetailPage(string elementId, SurmmaryDetailDTO data)
        {
            if(!IsDisplayCompleted)
            {
                Skip();
            }
            else
            {
                FocusToElement(elementId, data);
            }
                
        }
    }

    
}
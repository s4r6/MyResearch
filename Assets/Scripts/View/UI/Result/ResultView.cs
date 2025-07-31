using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Domain.Stage;
using NUnit.Framework;
using UnityEngine;
using View.Player;

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
            view.SetText(history.ObjectName, history.SelectedRiskLabel, history.ExecutedActionLabel, history.RiskChange.ToString(), history.ActionCost.ToString(), history.Explanation);
        }

        public void Display()
        {
            Debug.Log("�\��");
            this.gameObject.SetActive(true);

            Debug.Log("CursorLocked = false");
            PlayerView.cursorLocked = false;
            Cursor.lockState = CursorLockMode.None;    // �J�[�\�����R
            Cursor.visible = true;                     // �J�[�\���\��
        }

        public async UniTask ShowResult(List<RiskAssessmentHistory> histories)
        {
            foreach(RiskAssessmentHistory history in histories)
            {
                //�eHistory���쐬&�e�L�X�g�̓K�p
                var hist = Instantiate(HistoryViewPrefab, ParentTransform);
                var view = hist.GetComponent<HistroyView>();
                Histories.Add(view);
                SetHistory(view, history);

                //�A�j���[�V�������Ȃ���\��
                await view.Display();

                //�\�����I�������
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
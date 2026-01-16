using System.Collections.Generic;
using Domain.Stage;
using TMPro;
using UnityEngine;

namespace View.UI
{
    public class AllActionView : MonoBehaviour, IDetailWindow
    {
        
        [SerializeField]
        TMP_Text ExplanationText;
        [SerializeField]
        TMP_Text RiskLabelText;

        [SerializeField]
        List<GameObject> ActionLabels;
        [SerializeField]
        List<TMP_Text> ActionLabelTexts;

        [SerializeField]
        TMP_Text DisplayName;
        [SerializeField]
        TMP_Text RiskLabel;
        [SerializeField]
        TMP_Text ActionLabel;
        [SerializeField]
        TMP_Text RiskChange;
        [SerializeField]
        TMP_Text ActionCost;

        void Start()
        {
            foreach(var actionLabel in ActionLabels)
            {
                actionLabel.SetActive(false);
            }
        }

        void SetHeader(string name, string risklabel, string actionlabel, string risk, string action)
        {
            DisplayName.text = name;
            RiskLabel.text = risklabel;
            ActionLabel.text = actionlabel;
            RiskChange.text = $"リスク:{risk}";
            ActionCost.text = $"AP:-{action}";
        }

        public void SetData(SurmmaryDetailDTO data)
        {
            Debug.Log("対応策数:" + data.ActionLabels?.Count);
            

            ExplanationText.text = "";
            RiskLabelText.text = "";
            //初期化
            for (int i = data.ActionLabels.Count; i < 4; i++)
            {
                ActionLabelTexts[i].text = "";
                if (!ActionLabels[i].activeSelf) continue;
                ActionLabels[i].SetActive(false);
            }


            //データ設定
            SetHeader(data.DisplayName, data.RiskLabel, data.ActionLabel, data.RiskChange.ToString(), data.ActionCost.ToString());

            ExplanationText.text = data.Explanation;
            RiskLabelText.text = data.RiskLabel;

            if (data.ActionLabels?.Count <= 0 || data.ActionLabels == null) return;
            for (int i = 0; i < data.ActionLabels.Count; i++)
            {
                Debug.Log("有効化:" + data.ActionLabels[i].label);
                ActionLabelTexts[i].text = data.ActionLabels[i].label;
                ActionLabels[i].SetActive(true);
            }
        }
    }
}
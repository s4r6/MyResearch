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
            RiskChange.text = $"ÉäÉXÉN:{risk}";
            ActionCost.text = $"AP:-{action}";
        }

        public void SetData(SurmmaryDetailDTO data)
        {
            SetHeader(data.DisplayName, data.RiskLabel, data.ActionLabel, data.RiskChange.ToString(), data.ActionCost.ToString());

            ExplanationText.text = data.Explanation;
            RiskLabelText.text = data.RiskLabel;

            if (data.ActionLabels?.Count <= 0 || data.ActionLabels == null) return;
            for(int i = 0; i < data.ActionLabels.Count; i++)
            {
                ActionLabelTexts[i].text = data.ActionLabels[i].label;
                ActionLabels[i].SetActive(true);
            }
        }
    }
}
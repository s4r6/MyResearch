using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using System.Linq;
using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using Domain.Stage;

namespace View.UI
{
    public interface IDetailWindow
    {
        void SetData(SurmmaryDetailDTO data);
    }

    public class SelectedRiskView : MonoBehaviour, IDetailWindow
    {
        [SerializeField]
        GameObject Element;
        [SerializeField]
        GameObject Explanation;
        [SerializeField]
        GameObject Describe;
        [SerializeField]
        GameObject Risks;

        [SerializeField]
        TMP_Text ExplanationText;
        [SerializeField]
        TMP_Text DescribeText;
        [SerializeField]
        List<TMP_Text> RiskList;

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
            /*Explanation.SetActive(false);
            Describe.SetActive(false);
            Risks.SetActive(false);

            gameObject.SetActive(false);*/
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

            this.ExplanationText.text = data.Explanation;
            this.DescribeText.text = data.Description;

            if (data.RiskLabels?.Count <= 0 || data.RiskLabels == null) return;
            for(int i = 0; i < data.RiskLabels.Count; i++)
            {
                RiskList[i].text = data.RiskLabels[i];
            }
        }

        public UniTask DisplayAnimation()
        {
            Element.SetActive(true);
            Explanation.SetActive(true);
            Describe.SetActive(true);
            Risks.SetActive(true);

            return UniTask.CompletedTask;
        }
    }
}
using UnityEngine;
using UnityEngine.UI;

namespace View.UI
{
    public class HistroyView : MonoBehaviour
    {
        [SerializeField]
        Text ObjectId;
        [SerializeField]
        Text RiskLabel;
        [SerializeField]
        Text ActionLabel;
        [SerializeField]
        Text RiskReduce;
        [SerializeField]
        Text ActionCost;

        public void Initialize(string name, string risklabel, string actionlabel, string risk, string action)
        {
            ObjectId.text = name;
            RiskLabel.text = risklabel;
            ActionLabel.text = actionlabel;
            RiskReduce.text = $"リスク減少: {risk}";
            ActionCost.text = $"アクションポイント: -{action}";
        }
    }
}
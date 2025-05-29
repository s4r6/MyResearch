using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;

namespace View.UI
{
    public class HistroyView : MonoBehaviour
    {
        [SerializeField]
        RectTransform ObjectDatas;
        [SerializeField]
        Vector3 inPosition;
        [SerializeField] 
        Vector3 outPosition;
        [SerializeField]
        float duration = 0.5f;

        CanvasGroup ObjectCanvasGroup;

        [SerializeField]
        GameObject ActionResult;
        CanvasGroup ActionDatas;


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


        void Awake()
        {
            ObjectDatas.transform.position = outPosition;
            ObjectCanvasGroup = ObjectDatas.GetComponent<CanvasGroup>();
            ActionDatas = ActionResult.GetComponent<CanvasGroup>();

            ObjectCanvasGroup.alpha = 0;
            ActionDatas.alpha = 0;

            Hide();
        }

        public void SetText(string name, string risklabel, string actionlabel, string risk, string action)
        {
            ObjectId.text = name;
            RiskLabel.text = risklabel;
            ActionLabel.text = actionlabel;
            RiskReduce.text = $"リスク減少: {risk}";
            ActionCost.text = $"アクションポイント: -{action}";
        }

        public void Hide()
        {
            this.gameObject.SetActive(false);
        }

        public async UniTask Display()
        {
            this.gameObject.SetActive(true);
            await AnimationAsync();
        }

        async UniTask AnimationAsync()
        {
            // ObjectDatas のスライド＆フェード
            await UniTask.WhenAll(
                ObjectDatas.DOAnchorPos(inPosition, duration).SetEase(Ease.OutCubic).ToUniTask(),
                ObjectCanvasGroup.DOFade(1f, duration).SetEase(Ease.OutQuad).ToUniTask()
            );

            // ActionImage のフェードイン
            await ActionDatas.DOFade(1f, duration).SetEase(Ease.OutQuad).ToUniTask();
        }
    }
}
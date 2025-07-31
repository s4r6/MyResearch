using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using TMPro;

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

        [SerializeField]
        CanvasGroup DescriptionGroup;
        CanvasGroup ObjectCanvasGroup;

        [SerializeField]
        GameObject ActionResult;
        CanvasGroup ActionDatas;


        [SerializeField]
        TMP_Text ObjectId;
        [SerializeField]
        TMP_Text RiskLabel;
        [SerializeField]
        TMP_Text ActionLabel;
        [SerializeField]
        TMP_Text RiskReduce;
        [SerializeField]
        TMP_Text ActionCost;
        [SerializeField]
        TMP_Text Description;


        void Awake()
        {
            ObjectDatas.anchoredPosition = outPosition;
            ObjectCanvasGroup = ObjectDatas.GetComponent<CanvasGroup>();
            ActionDatas = ActionResult.GetComponent<CanvasGroup>();

            DescriptionGroup.alpha = 0;
            ObjectCanvasGroup.alpha = 0;
            ActionDatas.alpha = 0;

            Hide();
        }

        public void SetText(string name, string risklabel, string actionlabel, string risk, string action, string explanation)
        {
            ObjectId.text = name;
            RiskLabel.text = risklabel;
            ActionLabel.text = actionlabel;
            RiskReduce.text = $"リスク減少: {risk}";
            ActionCost.text = $"AP: -{action}";
            Description.text = explanation;
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
                DescriptionGroup.DOFade(1f, duration).SetEase(Ease.OutQuad).ToUniTask(),
                ObjectCanvasGroup.DOFade(1f, duration).SetEase(Ease.OutQuad).ToUniTask()
            );

            // ActionImage のフェードイン
            await ActionDatas.DOFade(1f, duration).SetEase(Ease.OutQuad).ToUniTask();
        }
    }
}
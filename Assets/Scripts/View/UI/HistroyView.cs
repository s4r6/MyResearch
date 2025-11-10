using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using TMPro;
using UseCase.Player;
using NUnit.Framework;
using System.Collections.Generic;

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
        GameObject ObjectDataObj;

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


        void Awake()
        {
            ObjectDatas.anchoredPosition = outPosition;
            ObjectCanvasGroup = ObjectDatas.GetComponent<CanvasGroup>();
            ActionDatas = ActionResult.GetComponent<CanvasGroup>();

            ObjectCanvasGroup.alpha = 0;
            ActionDatas.alpha = 0;

            Hide();
        }

        public void SetText(string name, string risklabel, string actionlabel, string risk, string action)
        {
            DisplayName.text = name;
            RiskLabel.text = risklabel;
            ActionLabel.text = actionlabel;
            RiskChange.text = $"リスク減少: {risk}";
            ActionCost.text = $"AP: -{action}";
        }

        public List<string> GetTextDatas()
        {
            List<string> datas = new();
            datas.Add(DisplayName.text);
            datas.Add(RiskLabel.text);
            datas.Add(ActionLabel.text);
            datas.Add(RiskChange.text);
            datas.Add(ActionCost.text);
            

            return datas;   
        }

        public string GetId() => DisplayName.text;

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

        public void SkipAnimation()
        {
            // アニメを全部終わった状態にする
            // ここでは「最終値を直接セット」でもいいし、DOTweenのCompleteでもいい
            ObjectDatas.anchoredPosition = inPosition;
            ObjectCanvasGroup.alpha = 1f;
            ActionDatas.alpha = 1f;

            DOTween.Kill(ObjectDatas);
            DOTween.Kill(ObjectCanvasGroup);
            DOTween.Kill(ActionDatas);

            gameObject.SetActive(true);
        }
    }
}
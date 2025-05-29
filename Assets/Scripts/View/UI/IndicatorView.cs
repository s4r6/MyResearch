using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace View.UI
{
    public class IndicatorView : MonoBehaviour
    {
        [SerializeField] private Image frontBar; // 明るい赤（即時反映）
        [SerializeField] private Image delayBar; // 暗めの赤（遅れて追従）
        [SerializeField] private float delaySpeed; // 秒で1.0fill分動く速度
        [SerializeField] float waitTime;
        [SerializeField] Text ValueText;

        private Tween currentTween;

        /// <summary>
        /// 値を設定し、遅延バーの完了まで待つ
        /// </summary>
        public async UniTask SetValueAsync(int current, int max)
        {
            if (max <= 0)
            {
                frontBar.fillAmount = 0f;
                delayBar.fillAmount = 0f;
                return;
            }

            float target = Mathf.Clamp01((float)current / max);
            frontBar.fillAmount = target;

            currentTween?.Kill();

            float diff = Mathf.Abs(delayBar.fillAmount - target);
            float duration = diff / delaySpeed;

            if (duration < 0.01f)
            {
                delayBar.fillAmount = target;
                return;
            }

            //ここで待機時間を追加
            if (waitTime > 0f)
                await UniTask.Delay(TimeSpan.FromSeconds(waitTime));

            currentTween = delayBar.DOFillAmount(target, duration)
                .SetEase(Ease.OutCubic);

            await currentTween.ToUniTask();
        }

        public void SetValue(int current, int max)
        {
            if (max <= 0)
            {
                frontBar.fillAmount = 0f;
                delayBar.fillAmount = 0f;
                return;
            }

            float target = Mathf.Clamp01((float)current / max);
            frontBar.fillAmount = target;
            delayBar.fillAmount = target;

            ValueText.text = $"{current}/{max}";
        }
    }
}
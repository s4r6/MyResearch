using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace View.UI
{
    public class ObjectInfoView : MonoBehaviour
    {
        [SerializeField]
        Text name;
        [SerializeField]
        Text describe;

        [SerializeField]
        RectTransform targetWindow;

        [SerializeField]
        float duration = 0.2f;
        //VIEW--------------------------------------------------------
        public void SetObjectInfo(string name, string describe)
        {
            this.name.text = name;
            this.describe.text = describe;
        }

        public void AnimateShowWindow()
        {
            targetWindow.localScale = Vector3.zero;
            targetWindow.gameObject.SetActive(true);

            targetWindow.DOScale(Vector3.one, duration)
                .SetEase(Ease.OutBack);
        }

        public void AnimateHideWindow()
        {
            targetWindow.DOScale(Vector3.zero, duration)
                .SetEase(Ease.OutBack)
                .OnComplete(() => targetWindow.gameObject.SetActive(false));
        }

        //PRESENTER--------------------------------------------------
        public void DisplayObjectInfoUI(string name, string describe)
        {
            SetObjectInfo(name, describe);
            AnimateShowWindow();
        }
    }

}

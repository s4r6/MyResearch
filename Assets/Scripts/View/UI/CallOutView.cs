using UnityEngine;
using UnityEngine.UI;

namespace View.UI
{
    public class CallOutView : MonoBehaviour
    {
        [SerializeField] public Text labelText;       // または TextMeshProUGUI
        [SerializeField] private Image underLine;
        [SerializeField] Text CostText;

        private bool isActive = false;
        private Color cachedColor;
        private RectTransform rectTransform;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();

            if (underLine != null)
                cachedColor = underLine.color;

            gameObject.SetActive(false);
        }

        /// <summary>
        /// UIとして位置とラベルを初期化する（Screen Space - Overlay対応）
        /// </summary>
        public void Initialize(string label, int actionCost, Vector2 anchoredPosition)
        {


            if (labelText == null)
            {
                return;
            }

            labelText.text = label;
            CostText.text = $"AP: -{actionCost}";

            if (rectTransform == null)
                rectTransform = GetComponent<RectTransform>();

            rectTransform.anchorMin = rectTransform.anchorMax = rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = anchoredPosition;

            isActive = true;
            gameObject.SetActive(true);
        }

        public void SetHighlighted(bool isHighlighted)
        {
            if (isHighlighted)
            {
                labelText.color = Color.red;
                CostText.color = Color.red;
                if (underLine != null) underLine.color = Color.red;
            }
            else
            {
                labelText.color = cachedColor;
                CostText.color = cachedColor;
                if (underLine != null) underLine.color = cachedColor;
            }
        }

        public void Activate()
        {
            isActive = true;
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            isActive = false;
            gameObject.SetActive(false);
        }
    }
}

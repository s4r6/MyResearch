using UnityEngine;
using UnityEngine.UI;

namespace View.UI
{
    public class CallOutView : MonoBehaviour
    {
        [SerializeField] public Text labelText;
        [SerializeField] private Image underLine;
        private Camera mainCamera;
        private bool isActive = false;

        Color cashColor;

        private void Start()
        {
            Debug.Log("[CallOutView] 初期化開始");
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("[CallOutView] メインカメラが見つかりません");
                return;
            }

            cashColor = underLine.color;

            Debug.Log("[CallOutView] メインカメラの参照を取得");
            gameObject.SetActive(false);
            Debug.Log("[CallOutView] 初期化完了");
        }

        private void Update()
        {
            if (isActive)
            {
                UpdateRotation();
            }
        }

        private void UpdateRotation()
        {
            if (mainCamera == null)
            {
                Debug.LogError("[CallOutView] メインカメラの参照が失われています");
                return;
            }
            transform.LookAt(mainCamera.transform);
            transform.Rotate(0, 180, 0);
        }

        public void Initialize(string label, Vector3 position)
        {
            Debug.Log($"[CallOutView] 初期化開始: ラベル={label}, 位置={position}");
            if (labelText == null)
            {
                Debug.LogError("[CallOutView] labelTextが設定されていません");
                return;
            }
            labelText.text = label;
            transform.position = position;
            isActive = true;
            gameObject.SetActive(true);
            Debug.Log("[CallOutView] 初期化完了");
        }

        public void SetHighlighted(bool isHighlighted)
        {
            if(isHighlighted)
            {
                labelText.color = Color.red;
                underLine.color = Color.red;
            }
            else
            {
                labelText.color = cashColor;
                underLine.color= cashColor;
            }
        }

        public void Deactivate()
        {
            Debug.Log("[CallOutView] 非アクティブ化開始");
            isActive = false;
            gameObject.SetActive(false);
            Debug.Log("[CallOutView] 非アクティブ化完了");
        }

        public void Activate()
        {
            isActive = true;
            gameObject.SetActive(true);
        }
    }
}

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System;
using View.Player;
using System.Linq;

namespace View.UI
{

    public enum ActionDirection
    {
        None,
        Up,
        Down,
        Left,
        Right,
        Confirm
    }
    public class ActionOverlayView : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private GameObject callOutUIPrefab;     // 吹き出しプレハブ
        [SerializeField] private Transform ParentTransform;           // Screen-Space Overlay Canvas

        [Header("Input")]
        [SerializeField] private PlayerInput input;              // InputSystem

        [Header("Layout")]
        [SerializeField] private float radius = 200f;            // 画面中心からのオフセット(px)

        [SerializeField]
        IndicatorView APIndicator;
        //--------------------------------------------------------------------------------
        // 内部フィールド
        //--------------------------------------------------------------------------------
        public readonly List<CallOutView> callOutCache = new();
        public readonly List<Vector2> availablePositions = new();   // 画面座標(anchoredPosition)

        public Action OnBackKeyPressed;
        public Action<ActionDirection> OnActionDirectionChanged;
        public Action OnSubmitKeyPressed;

        //--------------------------------------------------------------------------------
        // VIEW 初期化
        //--------------------------------------------------------------------------------
        private void Start()
        {

            // Overlay 親オブジェクト の子に 4 つ生成してキャッシュ
            // プレハブを 4 個生成
            for (int i = 0; i < 4; i++)
            {
                var go = Instantiate(callOutUIPrefab, ParentTransform);
                var view = go.GetComponent<CallOutView>();
                callOutCache.Add(view);
            }
            
            CalculatePositions();                    // 画面中心基準の位置配列を作成

            gameObject.SetActive(false);
        }
        
        public void Initialize(List<(string, int)> actions)
        {
            for (int i = 0; i < callOutCache.Count; i++)
            {
                if (i < actions.Count)
                {
                    // 中央(0.5,0.5)Pivot 想定。CallOutView.Initialize 内で anchoredPosition を設定。
                    callOutCache[i].Initialize(actions[i].Item1, actions[i].Item2, availablePositions[i]);
                    callOutCache[i].SetHighlighted(i == 0);
                }
                else
                {
                    callOutCache[i].Deactivate();
                }
            }
        }

        // 画面中心基準の 4 方向オフセットを計算
        void CalculatePositions()
        {
            // 優先順位：上 → 右 → 下 → 左（anchoredPosition）
            availablePositions.Clear();
            availablePositions.Add(new Vector2(0, ParentTransform.localPosition.y + radius));   // Up
            availablePositions.Add(new Vector2(ParentTransform.localPosition.x + radius, 0));     // Right
            availablePositions.Add(new Vector2(0, ParentTransform.localPosition.y - radius));    // Down
            availablePositions.Add(new Vector2(ParentTransform.localPosition.y - radius, 0));    // Left
        }

        // CallOut のハイライト更新
        public void UpdateCallOutHighlights(int index)
        {
            for (int i = 0; i < callOutCache.Count; i++)
            {
                if (callOutCache[i].gameObject.activeSelf)
                    callOutCache[i].SetHighlighted(i == index);
            }
        }

        // CallOuts を表示
        public void ShowCallOuts()
        {
            gameObject.SetActive(true);
            foreach (var view in callOutCache)
                if (view.gameObject.activeSelf) view.Activate();
        }

        // 非表示
        public void HideActionList()
        {
            foreach (var view in callOutCache) view.Deactivate();
            gameObject.SetActive(false);
        }

        // 選択中ラベル取得
        public string GetActionLabel(int index)
        {
            if (index < 0 || index >= callOutCache.Count) return null;
            if (!callOutCache[index].gameObject.activeSelf) return null;
            return callOutCache[index].labelText.text;
        }

        public void SetValueToIndicator(int current, int max)
        {
            APIndicator.SetValue(current, max);
        }

        #region InputEvents
        public void OnBackKey(InputAction.CallbackContext context)
        {
            if (!context.performed || !gameObject.activeSelf) return;
            OnBackKeyPressed?.Invoke();
        }

        public void OnSelect(InputAction.CallbackContext context)
        {
            if (!context.performed || !gameObject.activeSelf) return;

            Vector2 stick = context.ReadValue<Vector2>();
            if (stick == Vector2.zero) return;

            ActionDirection dir;
            // 縦横どちらか大きい方のみ採用（優先度：縦優先）
            if (Mathf.Abs(stick.y) >= Mathf.Abs(stick.x))
                dir = stick.y > 0 ? ActionDirection.Up : ActionDirection.Down;
            else
                dir = stick.x > 0 ? ActionDirection.Right : ActionDirection.Left;

            OnActionDirectionChanged?.Invoke(dir);
        }

        public void OnSubmit(InputAction.CallbackContext context)
        {
            if (!context.performed || !gameObject.activeSelf) return;
            OnSubmitKeyPressed?.Invoke();
        }
        #endregion
        
    }
}



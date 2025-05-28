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
        [SerializeField] private InputController inputController;// InputMap 切替管理

        [Header("Layout")]
        [SerializeField] private float radius = 200f;            // 画面中心からのオフセット(px)

        [SerializeField]
        IndicatorView APIndicator;
        //--------------------------------------------------------------------------------
        // 内部フィールド
        //--------------------------------------------------------------------------------
        private readonly List<CallOutView> callOutCache = new();
        private readonly List<Vector2> availablePositions = new();   // 画面座標(anchoredPosition)

        public Action OnBackKeyPressed;
        public Action<ActionDirection> OnActionDirectionChanged;
        public Action OnSubmitKeyPressed;

        //--------------------------------------------------------------------------------
        // VIEW 初期化
        //--------------------------------------------------------------------------------
        private void Start()
        {

            InitializeCallOutCache();                // プレハブを 4 個生成
            CalculatePositions();                    // 画面中心基準の位置配列を作成

            gameObject.SetActive(false);
        }

        private void InitializeCallOutCache()
        {
            // Overlay 親オブジェクト の子に 4 つ生成してキャッシュ
            for (int i = 0; i < 4; i++)
            {
                var go = Instantiate(callOutUIPrefab, ParentTransform);
                var view = go.GetComponent<CallOutView>();
                callOutCache.Add(view);
            }
        }

        //--------------------------------------------------------------------------------
        // Input ハンドラ
        //--------------------------------------------------------------------------------
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

        //--------------------------------------------------------------------------------
        // 画面中心基準の 4 方向オフセットを計算
        //--------------------------------------------------------------------------------
        private void CalculatePositions()
        {


            // 優先順位：上 → 右 → 下 → 左（anchoredPosition）
            availablePositions.Clear();
            availablePositions.Add(new Vector2(0, ParentTransform.localPosition.y + radius));   // Up
            availablePositions.Add(new Vector2(ParentTransform.localPosition.x + radius, 0));     // Right
            availablePositions.Add(new Vector2(0, ParentTransform.localPosition.y - radius));    // Down
            availablePositions.Add(new Vector2(ParentTransform.localPosition.y - radius, 0));    // Left
        }

        //--------------------------------------------------------------------------------
        // CallOut のハイライト更新
        //--------------------------------------------------------------------------------
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
        public string? GetCurrentSelectActionLabel(int index)
        {
            if (index < 0 || index >= callOutCache.Count) return null;
            if (!callOutCache[index].gameObject.activeSelf) return null;
            return callOutCache[index].labelText.text;
        }

        public void SetValueToIndicator(int current, int max)
        {
            APIndicator.SetValue(current, max);
        }
        //--------------------------------------------------------------------------------
        // PRESENTER ロジック
        //--------------------------------------------------------------------------------
        private int currentSelectedIndex = 0;
        private Action<int?> OnEndActionView;

        public void StartSelectAction(int remainingActionPoint, int maxActionPoint, List<(string, int)> actions, string targetObjectId, Action<int?> onEnd)
        {
            OnEndActionView = onEnd;

            OnBackKeyPressed += OnCancelSelectAction;
            OnActionDirectionChanged += HandleActionDirection;
            OnSubmitKeyPressed += OnActionSelected;

            // TODO: targetObjectId で何かしたい場合はここに処理を書く
            // basePosition = GameObject.Find(targetObjectId).transform.position;

            EnableUIInput();
            UpdateCallOuts(actions);
            ShowCallOuts();
            SetValueToIndicator(remainingActionPoint, maxActionPoint);
        }

        public void OutPutLog()
        {
            Debug.Log("ポイントが足りません。");
            EndSelectAction();
        }

        public void EndSelectAction()
        {
            DisableUIInput();
            HideActionList();

            OnBackKeyPressed -= OnCancelSelectAction;
            OnActionDirectionChanged -= HandleActionDirection;
            OnSubmitKeyPressed -= OnActionSelected;

            OnEndActionView = null;
        }

        private void OnActionSelected()
        {

            OnEndActionView?.Invoke(currentSelectedIndex);
        }

        private void OnCancelSelectAction()
        {
            OnEndActionView?.Invoke(null);
        }

        // 選択肢ラベルと位置を更新
        private void UpdateCallOuts(List<(string, int)> actions)
        {
            currentSelectedIndex = 0;

            for (int i = 0; i < callOutCache.Count; i++)
            {
                if (i < actions.Count)
                {
                    // 中央(0.5,0.5)Pivot 想定。CallOutView.Initialize 内で anchoredPosition を設定。
                    callOutCache[i].Initialize(actions[i].Item1, actions[i].Item2, availablePositions[i]);
                    callOutCache[i].SetHighlighted(i == currentSelectedIndex);
                }
                else
                {
                    callOutCache[i].Deactivate();
                }
            }
        }

        //--------------------------------------------------------------------------------
        // 入力マップ切替
        //--------------------------------------------------------------------------------
        private void EnableUIInput()
        {
            inputController.SwitchActionMapToUI();

        }

        private void DisableUIInput()
        {
            inputController.SwitchActionMapToPlayer();
        }

        //--------------------------------------------------------------------------------
        // 方向入力ハンドラ
        //--------------------------------------------------------------------------------
        public void HandleActionDirection(ActionDirection dir)
        {
            int next = currentSelectedIndex;

            switch (dir)
            {
                case ActionDirection.Up: next = 0; break;
                case ActionDirection.Right: next = 1; break;
                case ActionDirection.Down: next = 2; break;
                case ActionDirection.Left: next = 3; break;
                case ActionDirection.Confirm:
                    OnActionSelected();
                    return;
            }

            // 存在する CallOut だけに移動
            if (next < callOutCache.Count && callOutCache[next].gameObject.activeSelf)
            {
                currentSelectedIndex = next;
                UpdateCallOutHighlights(currentSelectedIndex);
            }
        }
    }
}



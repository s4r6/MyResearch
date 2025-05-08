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
        [SerializeField] private GameObject callOutUIPrefab;
        [SerializeField] private GameObject worldSpaceCanvas;
        [SerializeField] private PlayerInput input;
        [SerializeField] private InputController inputController;
        [SerializeField] private float radius = 3.0f;

        private Camera mainCamera;
        private Vector3 basePosition;
        private List<CallOutView> callOutCache = new();
        private Vector3[] positions;
        
        public Action OnActionKeyPressed;
        public Action<ActionDirection> OnActionDirectionChanged;
        public Action OnSubmitKeyPressed;

        //-------------------------VIEW----------------------------
        private void Start()
        {
            Debug.Log("[ActionOverlayView] 初期化開始");
            mainCamera = Camera.main;
            InitializeCallOutCache();
            Debug.Log("[ActionOverlayView] 初期化完了");
        }

        private void InitializeCallOutCache()
        {
            for (int i = 0; i < 4; i++)
            {
                var callOutUI = Instantiate(callOutUIPrefab, Vector3.zero, Quaternion.identity, worldSpaceCanvas.transform);
                callOutCache.Add(callOutUI.GetComponent<CallOutView>());
            }
        }

        public void OnActionKey(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;
            OnActionKeyPressed?.Invoke();
        }

        public void OnSelect(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            var input = context.ReadValue<Vector2>();
            if (input == Vector2.zero)
                return;

            ActionDirection direction;
            // 縦 or 横いずれかのみ許可（片方優先）
            if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            {
                direction = input.x > 0 ? ActionDirection.Right : ActionDirection.Left;
            }
            else
            {
                direction = input.y > 0 ? ActionDirection.Up : ActionDirection.Down;
            }

            Debug.Log("選択肢変更:" + direction);
            OnActionDirectionChanged?.Invoke(direction);
        }

        public void OnSubmit(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            OnSubmitKeyPressed?.Invoke();
        }

        private void CalculatePositions()
        {
            Debug.Log("[ActionOverlayView] 位置計算開始");
            var rightOffset = mainCamera.transform.right * radius;
            var upOffset = mainCamera.transform.up * radius;
            var leftOffset = -mainCamera.transform.right * radius;
            var downOffset = -mainCamera.transform.up * radius;

            positions = new Vector3[] {
                basePosition + rightOffset,
                basePosition + upOffset,
                basePosition + downOffset,
                basePosition + leftOffset
            };
            Debug.Log("[ActionOverlayView] 位置計算完了");
        }

        public void UpdateCallOutHighlights(int index)
        {
            for (int i = 0; i < callOutCache.Count; i++)
            {
                if (callOutCache[i].gameObject.activeSelf)
                {
                    callOutCache[i].SetHighlighted(i == index);
                }
            }
        }

        public void ShowCallOuts()
        {
            Debug.Log("[ActionOverlayView] CallOuts表示");
            foreach (var callOut in callOutCache)
            {
                if (callOut.gameObject.activeSelf)
                {
                    callOut.Activate();
                }
            }
        }

        public void HideActionList()
        {
            DisableUIInput();
            foreach (var callOut in callOutCache)
            {
                callOut.Deactivate();
            }
            Debug.Log("[ActionOverlayView] アクションリスト非表示完了");
        }

        public string? GetCurrentSelectActionLabel(int index)
        {
            if (callOutCache[index].gameObject.activeSelf == true)
            {
                return callOutCache[index].labelText.text;
            }

            return null;

        }


        //-------------------------PRESENTER----------------------------
        
        int currentSelectedIndex = 0;
        Action<string?> OnEndActionView;
        public void StartSelectAction(List<string> actionLabels, string targetObjectId, Action<string?> onEnd)
        {
            OnEndActionView = onEnd;
            OnActionKeyPressed += OnCancelSelectAction;
            OnActionDirectionChanged += HandleActionDirection;
            OnSubmitKeyPressed += OnActionSelected;


            Debug.Log($"[ActionOverlayView] アクションリスト表示開始: {actionLabels.Count}個のアクション");
            FindPosition(targetObjectId);
            CalculatePositions();

            EnableUIInput();
            UpdateCallOuts(actionLabels);
            ShowCallOuts();
            Debug.Log("[ActionOverlayView] アクションリスト表示完了");
        }

        public void EndSelectAction()
        {
            DisableUIInput();
            HideActionList();

            OnActionKeyPressed -= OnCancelSelectAction;
            OnActionDirectionChanged -= HandleActionDirection;
            OnSubmitKeyPressed -= OnActionSelected;

            OnEndActionView = null;
        }

        void OnActionSelected()
        {
            string label = GetCurrentSelectActionLabel(currentSelectedIndex);
            OnEndActionView?.Invoke(label);
        }

        void OnCancelSelectAction()
        {
            OnEndActionView?.Invoke(null);
        }

        private void UpdateCallOuts(List<string> labels)
        {
            for (int i = 0; i < callOutCache.Count; i++)
            {
                if (i < labels.Count)
                {
                    callOutCache[i].Initialize(labels[i], positions[i]);
                    callOutCache[i].SetHighlighted(i == currentSelectedIndex);
                }
                else
                {
                    callOutCache[i].Deactivate();
                }
            }
        }

        private void FindPosition(string targetObjectId)
        {
            Debug.Log($"[ActionOverlayView] 対象オブジェクトの位置を検索: {targetObjectId}");
            basePosition = GameObject.Find(targetObjectId).transform.position;
            Debug.Log($"[ActionOverlayView] 位置を取得: {basePosition}");
        }

        

        private void EnableUIInput()
        {
            inputController.SwitchActionMapToUI();
            Debug.Log("[ActionOverlayView] UI入力有効化");
        }

        private void DisableUIInput()
        {
            inputController.SwitchActionMapToPlayer();
            Debug.Log("[ActionOverlayView] UI入力無効化");
        }

        public void HandleActionDirection(ActionDirection direction)
        {
            switch (direction)
            {
                case ActionDirection.Up:
                    currentSelectedIndex = 1;
                    break;
                case ActionDirection.Down:
                    currentSelectedIndex = 2;
                    break;
                case ActionDirection.Left:
                    currentSelectedIndex = 3;
                    break;
                case ActionDirection.Right:
                    currentSelectedIndex = 0;
                    break;
                case ActionDirection.Confirm:
                    OnActionSelected(currentSelectedIndex);
                    break;
            }

            Debug.Log("ハイライト変更:" + currentSelectedIndex);
            UpdateCallOutHighlights(currentSelectedIndex);
        }



        private void OnActionSelected(int index)
        {
            Debug.Log($"[ActionOverlayView] アクション選択: {index}");
        }
    }
}



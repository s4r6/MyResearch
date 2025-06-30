using System;
using System.Collections.Generic;
using UseCase.Player;
using View.Player;
using View.UI;
using UnityEngine;
using Domain.Action;

namespace Presenter.Player
{
    public class ActionPresenter : IActionPresenter
    {
        ActionOverlayView view;
        InputController inputController;

        private int currentIndex = 0;
        private Action<string> OnEndActionView;


        public ActionPresenter(ActionOverlayView view, InputController inputController)
        {
            this.view = view;
            this.inputController = inputController;
        }

        public void StartSelectAction(int remainingActionPoint, int maxActionPoint, List<(string, int)> actions, string targetObjectId, Action<string> onEnd)
        {
            OnEndActionView = onEnd;

            //コールバック登録
            view.OnBackKeyPressed += OnCancelSelectAction;
            view.OnActionDirectionChanged += HandleActionDirection;
            view.OnSubmitKeyPressed += OnActionSelected;

            //UI入力を有効化
            EnableUIInput();
            //コールアウトを初期化
            InitializeCallOuts(actions);
            view.ShowCallOuts();
            view.SetValueToIndicator(remainingActionPoint, maxActionPoint);
        }

        public void EndSelectAction()
        {
            DisableUIInput();
            view.HideActionList();

            view.OnBackKeyPressed -= OnCancelSelectAction;
            view.OnActionDirectionChanged -= HandleActionDirection;
            view.OnSubmitKeyPressed -= OnActionSelected;

            OnEndActionView = null;
        }

        public void OutPutLog()
        {
            Debug.Log("ActionPointが足りません");
            EndSelectAction();
        }

        private void OnActionSelected()
        {
            OnEndActionView?.Invoke(view.GetActionLabel(currentIndex));
            EndSelectAction();
        }

        private void OnCancelSelectAction()
        {
            OnEndActionView?.Invoke(string.Empty);
            EndSelectAction();
        }

        // 選択肢ラベルと位置を更新
        void InitializeCallOuts(List<(string, int)> actions)
        {
            currentIndex = 0;
            view.Initialize(actions);
        }


        void HandleActionDirection(ActionDirection dir)
        {
            int next = currentIndex;

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
            if (next < view.callOutCache.Count && view.callOutCache[next].gameObject.activeSelf)
            {
                currentIndex = next;
                view.UpdateCallOutHighlights(currentIndex);
            }
        }


        void EnableUIInput()
        {
            inputController.SwitchActionMapToUI();

        }

        void DisableUIInput()
        {
            inputController.SwitchActionMapToPlayer();
        }
    }
}

using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System;
using UnityEngine;
using UseCase.Player;
using View.Player;
using View.UI;

namespace Presenter.Player
{
    public class InspectPresenter : IInspectPresenter
    {
        InputController inputController;
        ObjectInfoView view;

        int currentIndex = 0;
        Action<string> OnEndInspectView;

        public InspectPresenter(InputController inputController, ObjectInfoView view)
        {
            this.inputController = inputController;
            this.view = view;
        }

        void OnChoiceSelected()
        {
            OnEndInspectView?.Invoke(view.GetButtonLabel(currentIndex));
            EndInspect();
        }

        void OnCancelInspect()
        {
            OnEndInspectView?.Invoke(string.Empty);
            EndInspect();
        }

        void MoveSelection(int delta)
        {
            int max = 4;
            currentIndex = Mathf.Clamp(currentIndex - delta, 0, max - 1);
            view.HighlightButton(currentIndex);
        }

        public async UniTask StartInspect(InspectData data, Action<string> onEnd)
        {
            OnEndInspectView = onEnd;

            //Viewの入力イベント登録
            if (data.ChoiceLabels != null && data.IsSelectable)
            {
                view.OnSubmitEvent += OnChoiceSelected;
                view.OnScrollEvent += MoveSelection;
            }
            view.OnBackEvent += OnCancelInspect;

            //UIでの入力を有効化
            EnableUIInput();
            
            //Viewに表示する情報を渡す
            view.SetObjectInfo(data.DisplayName, data.Description);
            if(data.ChoiceLabels != null)
                view.SetChoices(data.ChoiceLabels);
            else
                view.HideButtons();

            //Viewの表示アニメーション
            await view.AnimateShowWindow();

            if (data.ChoiceLabels != null)
            {
                //選択肢のハイライト表示
                currentIndex = CalculateIndex(data.SelectedLabel, data.ChoiceLabels);
                view.HighlightButton(currentIndex);
            }
        }

        /*public async UniTask DisplayLabels(string name, string describe, int selectedIndex, List<string> ChoiceTexts, Action<string> onEnd)
        {
            OnEndInspectView = onEnd;

            view.OnBackEvent += OnCancelInspect;

            EnableUIInput();
            view.SetObjectInfo(name, describe);
            view.SetChoices(ChoiceTexts);
            await view.AnimateShowWindow();
            currentIndex = selectedIndex;
            view.HighlightButton(selectedIndex);
        }*/

        public void EndInspect()
        {
            DisableUIInput();
            view.AnimateHideWindow();
            view.ResetText();
            view.DisplayButtons();

            view.OnSubmitEvent -= OnChoiceSelected;
            view.OnBackEvent -= OnCancelInspect;
            view.OnScrollEvent -= MoveSelection;
            OnEndInspectView = null;
        }

        int CalculateIndex(string selectedLable, List<string> choiceLabels)
        {
            var index = choiceLabels.FindIndex(x => x == selectedLable);
            index = index < 0 ? 0 : index;

            return index;
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
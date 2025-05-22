using System;
using System.Collections.Generic;
using DG.Tweening;
using Domain.Stage.Object;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using View.Player;

namespace View.UI
{
    public class ObjectInfoView : MonoBehaviour
    {
        [SerializeField]
        Text objectName;
        [SerializeField]
        Text describe;

        [SerializeField]
        RectTransform targetWindow;

        [SerializeField]
        float duration = 0.2f;

        [SerializeField]
        List<Button> buttons;
        [SerializeField]
        List<Text> buttonTexts;

        [SerializeField]
        PlayerInput input;
        [SerializeField]
        InputController inputController;
        public Action OnSubmitEvent;
        public Action OnBackEvent;
        public Action<int> OnScrollEvent;

        Color selectedColor = (Color)new Color32(100, 224, 150, 255);
        Color normalColor = Color.white;
        //VIEW--------------------------------------------------------
        void Start()
        {
            //初期の色をキャッシュ
            normalColor = buttons[0].colors.normalColor ;
            UnityEngine.ColorUtility.TryParseHtmlString("#017B69", out selectedColor);
            gameObject.SetActive(false);
        }
        public void SetObjectInfo(string name, string describe)
        {
            this.objectName.text = name;
            this.describe.text = describe;
        }

        public void SetChoices(List<string> texts)
        {
            for (int i = 0; i < texts.Count; i++)
            {
                //Debug.Log($"Lable:{texts[i]}, i:{i}");
                buttonTexts[i].text = texts[i];
            }
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

        public void HighlightButton(int index)
        {
            for (int i = 0; i < buttons.Count; i++) 
            {
                var colors = buttons[i].colors;
                colors.selectedColor = (i == index) ? selectedColor : normalColor;
                buttons[i].colors = colors;
            }

            EventSystem.current.SetSelectedGameObject(buttons[index].gameObject);
        }

        public void OnSubmit(InputAction.CallbackContext context)
        {
            if (!context.performed || !gameObject.activeSelf) return;
            OnSubmitEvent?.Invoke();
        }

        public void OnBack(InputAction.CallbackContext context)
        {
            if (!context.performed || !gameObject.activeSelf) return;
            OnBackEvent?.Invoke();
        }

        int lastFrame = -1;
        public void OnScroll(InputAction.CallbackContext context)
        {
            if (Time.frameCount == lastFrame) return;
            
            var vec = context.ReadValue<Vector2>();
            var delta = (int)vec.y;
            
            if (delta == 0)
                return;

            Debug.Log(delta);
            lastFrame = Time.frameCount;
            OnScrollEvent?.Invoke(delta);
        }

        public void ResetText()
        {
            objectName.text = string.Empty;
            describe.text = string.Empty;
            foreach(var text in buttonTexts)
            {
                text.text = string.Empty;
            }
        }

        //PRESENTER--------------------------------------------------

        int currentIndex = 0;
        Action<string?> OnEndInspectView;

        void OnChoiceSelected()
        {
            var selectedChoiceText = buttonTexts[currentIndex].text;
            OnEndInspectView?.Invoke(selectedChoiceText);
        }

        void OnCancelInspect()
        {
            OnEndInspectView?.Invoke(null);
        }

        void MoveSelection(int delta)
        {
            int max = buttons.Count;
            currentIndex = Mathf.Clamp(currentIndex - delta, 0, max - 1);
            HighlightButton(currentIndex);
        }

        public void StartInspect(string name, string describe, List<string> ChoiceTests, Action<string?> onEnd)
        {
            OnEndInspectView = onEnd;

            OnSubmitEvent += OnChoiceSelected;
            OnBackEvent += OnCancelInspect;
            OnScrollEvent += MoveSelection;

            EnableUIInput();
            SetObjectInfo(name, describe);
            SetChoices(ChoiceTests);
            AnimateShowWindow();
        }

        public void EndInspect()
        {
            DisableUIInput();
            AnimateHideWindow();
            ResetText();

            OnSubmitEvent -= OnChoiceSelected;
            OnBackEvent -= OnCancelInspect;
            OnScrollEvent -= MoveSelection;

            OnEndInspectView = null;
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

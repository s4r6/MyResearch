using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Domain.Stage.Object;
using TMPro;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UseCase.Player;
using View.Player;

namespace View.UI
{
    public class ObjectInfoView : MonoBehaviour
    {
        [SerializeField]
        TMP_Text objectName;
        [SerializeField]
        TMP_Text describe;

        [SerializeField]
        RectTransform targetWindow;

        [SerializeField]
        float duration = 0.2f;

        [SerializeField]
        List<Button> buttons;
        [SerializeField]
        List<TMP_Text> buttonTexts;

        [SerializeField]
        PlayerInput input;
        
        public Action OnSubmitEvent;
        public Action OnBackEvent;
        public Action<int> OnScrollEvent;

        Color selectedColor = (Color)new Color32(100, 224, 150, 255);
        Color normalColor = Color.white;
        //-----------------------------------VIEW-----------------------------------
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

        public async UniTask AnimateShowWindow()
        {
            targetWindow.localScale = Vector3.zero;
            targetWindow.gameObject.SetActive(true);

            await targetWindow.DOScale(Vector3.one, duration)
                .SetEase(Ease.OutBack).ToUniTask();

            //Cursor.lockState = CursorLockMode.None;
        }

        public void AnimateHideWindow()
        {
            //Cursor.lockState = CursorLockMode.Locked;

            targetWindow.DOScale(Vector3.zero, duration)
                .SetEase(Ease.OutBack)
                .OnComplete(() => targetWindow.gameObject.SetActive(false));
        }

        public void HighlightButton(int index, bool IsSelectable = true)
        {
            if (IsSelectable)
            {
                for (int i = 0; i < buttons.Count; i++)
                {
                    var colors = buttons[i].colors;
                    colors.normalColor = (i == index) ? selectedColor : normalColor;
                    buttonTexts[i].color = Color.white;
                    buttons[i].colors = colors;
                }
            }
            else
            {
                for (int i = 0; i < buttons.Count; i++)
                {
                    var colors = buttons[i].colors;
                    colors.normalColor = (i == index) ? selectedColor : normalColor;
                    buttonTexts[i].color = (i == index) ? Color.red : Color.gray;
                    buttons[i].colors = colors;
                    
                }
            }
            

            //EventSystem.current.SetSelectedGameObject(buttons[index].gameObject);
        }

        public string GetButtonLabel(int index)
        {
            return buttonTexts[index].text;
        }

        public void OnSubmit(InputAction.CallbackContext context)
        {
            if (!context.performed || !gameObject.activeSelf) return;
            if (OnSubmitEvent != null)
            {
                OnSubmitEvent?.Invoke();
            }
            else
            {
                Debug.Log("何もしない");
                // SubmitEventが無ければ、イベントを無視し、選択状態を維持する
                // EventSystemによる"デフォーカス"を防ぐ
                //EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject);
            }
            
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

            lastFrame = Time.frameCount;
            OnScrollEvent?.Invoke(delta);
        }

        public void OnArrow(InputAction.CallbackContext context)
        {
            if (!context.performed || !gameObject.activeSelf) return;

            int delta = (int)context.ReadValue<Vector2>().y;
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

        public void HideButtons()
        {
            foreach(var button in buttons)
            {
                button.gameObject.SetActive(false);
            }
        }

        public void DisplayButtons()
        {
            foreach(var button in buttons)
            {
                button.gameObject.SetActive(true);
            }
        }

        
    }

}

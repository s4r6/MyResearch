using System;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UniRx;
using Presenter.Tutorial;
using System.Threading;

namespace View.Tutorial
{

    public class TutorialWindowView : MonoBehaviour, ITutorialWindowView
    {
        [SerializeField] private TMP_Text messageText;
        [SerializeField] private TMP_Text hintText;

        [SerializeField]
        GameObject hintWindow;
        void Awake()
        {
            gameObject.SetActive(false);
            hintWindow.SetActive(false);
        }

        public void Show(string message, string uiHint)
        {
            if (messageText != null)
                messageText.text = message;

            if (hintText != null)
            {
                hintText.text = uiHint ?? string.Empty;
                if(uiHint != string.Empty)
                {
                    hintWindow.SetActive(true);
                }
            }
                

            gameObject.SetActive(true);
        }

        public  void HideHint()
        {
            hintText.text = string.Empty;
            hintWindow.SetActive(false);
        }

        public async UniTask HideAsync(CancellationToken token = default)
        {
            await UniTask.Yield(token);
            gameObject.SetActive(false);
        }
    }
}
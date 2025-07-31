using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using View.Player;

namespace View.UI
{
    public class DocumentView : MonoBehaviour
    {
        //--------------------------VIEW-------------------------------
        [Header("Page Root (Single)")]
        [SerializeField] private RectTransform pageRoot;

        [Header("Displayed Page")]
        [SerializeField] private Image pageImage;

        [Header("Animation Root")]
        [SerializeField] private RectTransform viewRoot;

        [Header("Animation Settings")]
        [SerializeField] private float openDuration = 0.3f;
        [SerializeField] private float closeDuration = 0.25f;
        [SerializeField] private Ease easeOpen = Ease.OutBack;
        [SerializeField] private Ease easeClose = Ease.InBack;

        [SerializeField]
        InputController inputController;

        private bool isOpen = false;
        private Tweener currentTween;

        [SerializeField]
        private Sprite[] pages = Array.Empty<Sprite>();

        public Action OnBackEvent;
        public Action<int> OnPageMoveEvent;

        private void Awake()
        {
            viewRoot.localScale = Vector3.zero;
            gameObject.SetActive(false);
        }

        public void InitializePages(Sprite[] pageSprites)
        {
            pages = pageSprites;
        }

        public async UniTask OpenAsync()
        {
            if (isOpen) return;
            isOpen = true;
            gameObject.SetActive(true);

            currentTween?.Kill();
            viewRoot.localScale = Vector3.zero;
            currentTween = viewRoot.DOScale(Vector3.one, openDuration).SetEase(easeOpen);
            await currentTween.AsyncWaitForCompletion();
        }

        public async UniTask CloseAsync()
        {
            if (!isOpen) return;
            isOpen = false;

            currentTween?.Kill();
            currentTween = viewRoot.DOScale(Vector3.zero, closeDuration).SetEase(easeClose);
            await currentTween.AsyncWaitForCompletion();

            gameObject.SetActive(false);
        }
        public void OnCloseInput() => _ = CloseAsync();

        private void UpdatePage(int index)
        {
            if (pages.Length == 0 || index >= pages.Length)
            {
                pageImage.sprite = null;
                return;
            }

            pageImage.sprite = pages[index];
            //pageImage.SetNativeSize();
        }

        public void OnBack(InputAction.CallbackContext context)
        {
            if (!context.performed || !gameObject.activeSelf) return;
            Debug.Log("CloseDocument");
            OnBackEvent?.Invoke();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            if (!context.performed || !gameObject.activeSelf) return;

            var input = context.ReadValue<float>();
            Debug.Log(input);
            var delta = (int)input;

            if (delta == 0) return;

            OnPageMoveEvent?.Invoke(delta);
        }

        //------------------------PRESENTER--------------------------------

        Action OnCloseDocument;
        int currentIndex = 0;

        void OnClose()
        {
            OnCloseDocument?.Invoke();
        }

        void MovePage(int delta)
        {
            int max = pages.Length;
            currentIndex = Mathf.Clamp(currentIndex + delta, 0, max - 1);
            UpdatePage(currentIndex);
        }

        public void DisplayDocument(Action onCloseDocument)
        {
            OnCloseDocument = onCloseDocument;

            OnBackEvent += OnClose;
            OnPageMoveEvent += MovePage;

            OpenAsync().Forget();
            EnableUIInput();
        }

        public async UniTask HideDocument()
        {
            OnBackEvent -= OnClose;
            OnPageMoveEvent -= MovePage;

            DisableUIInput();
            await CloseAsync();

            OnCloseDocument = null;
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
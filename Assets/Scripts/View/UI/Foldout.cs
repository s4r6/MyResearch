using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace View.UI
{
    public class Foldout : MonoBehaviour
    {
        [SerializeField]
        Button toggleButton;
        [SerializeField]
        GameObject foldoutContent;
        [SerializeField]
        LayoutElement foldoutLayout;

        bool isOpen = false;

        private void Awake()
        {
            foldoutLayout.ignoreLayout = true;
            foldoutContent.SetActive(false);
        }

        private void Start()
        {
            toggleButton.onClick.AddListener(ToggleFoldout);

            DelayLayoutRebuild().Forget();
        }

        void ToggleFoldout()
        {
            isOpen = !isOpen;
            foldoutLayout.ignoreLayout = !isOpen;
            foldoutContent.SetActive(isOpen);

            DelayLayoutRebuild().Forget();
        }

        async UniTaskVoid DelayLayoutRebuild()
        {
            await UniTask.Yield();
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
        }

        private void OnDestroy()
        {
            toggleButton.onClick.RemoveListener(ToggleFoldout);
        }
    }
}
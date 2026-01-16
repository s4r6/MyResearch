using Cysharp.Threading.Tasks;
using Presenter.Sound;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UseCase.Title;
using View.Sound;

namespace View.Title
{
    public class ModeSelectView : MonoBehaviour, IModeSelectView
    {
        [SerializeField]
        TMP_Text warningText;

        [SerializeField]
        InputField playerNameField;
        [SerializeField]
        Button singlePlayerButton;
        [SerializeField]
        Button multiPlayerButton;
        [SerializeField]
        Button tutorialButton;

        [SerializeField]
        SoundView sound;

        TitleUseCase usecase;

        private void Awake()
        {
            playerNameField.onEndEdit.AddListener(OnEndEdit);
            singlePlayerButton.onClick.AddListener(OnSinglePlayerButtonPressed);
            multiPlayerButton.onClick.AddListener(() => OnMultiPlayerButtonPressed().Forget());
            tutorialButton.onClick.AddListener(OnTutorialButtonPressed);
        }

        void Start()
        {
            warningText.gameObject.SetActive(false);
        }

        public void Injection(TitleUseCase usecase)
        {
            this.usecase = usecase;
        }

        public void WarningInputName()
        {
            warningText.gameObject.SetActive(true);
        }

        void OnTutorialButtonPressed()
        {
            sound.PlaySE(AudioId.ButtonClick, 1f);
            usecase.ChangeGameMode(GameMode.Tutorial).Forget();
        }

        void OnSinglePlayerButtonPressed()
        {
            sound.PlaySE(AudioId.ButtonClick, 1f);
            usecase.ChangeGameMode(GameMode.Solo).Forget();
        }

        async UniTask OnMultiPlayerButtonPressed()
        {
            sound.PlaySE(AudioId.ButtonClick, 1f);
            await usecase.ChangeGameMode(GameMode.Multi);
        }

        void OnEndEdit(string text)
        {
            usecase.OnPlayerNameInputed(text);
        }

        public void Activate()
        {
            warningText.gameObject.SetActive(false);
            gameObject.SetActive(true);
        }

        public void DeActivate()
        {
            gameObject?.SetActive(false);
        }
    }
}
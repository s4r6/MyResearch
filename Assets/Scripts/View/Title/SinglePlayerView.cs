using Presenter.Sound;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UseCase.Title;
using View.Sound;

namespace View.Title
{
    public class SinglePlayerView : MonoBehaviour, ISinglePlayerView
    {
        [SerializeField]
        Button startButton;
        [SerializeField]
        Button backButton;

        [SerializeField]
        Text playerName;

        [SerializeField]
        SoundView sound;

        TitleUseCase usecase;

        int StageId = 1;

        void Start()
        {
            backButton.onClick.AddListener(OnBackButtonPressed);
            startButton.onClick.AddListener(OnStartButtonPressed);
        }

        public void Injection(TitleUseCase usecase)
        {
            this.usecase = usecase;
        }

        public void Activate()
        {
            gameObject.SetActive(true);
        }

        public void DeActivate()
        {
            gameObject.SetActive(false);
        }

        void OnBackButtonPressed()
        {
            sound.PlaySE(AudioId.ButtonClick, 1f);
            usecase.ChangeGameMode(GameMode.ModeSelect);
        }

        void OnStartButtonPressed()
        {
            sound.PlaySE(AudioId.ButtonClick, 1f);
            usecase.StartGame(StageId);
        }

        public void OnStageButtonPressed(int stageId)
        {
            StageId = stageId;
        }

        public void SetPlayerName(string playerName)
        {
            this.playerName.text = playerName;
        }

        public void TransitionInGame(int stageId)
        {
            SceneManager.LoadScene("Stage" + stageId);
        }
    }
}
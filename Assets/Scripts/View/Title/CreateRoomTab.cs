using NUnit.Framework.Internal;
using Presenter.Sound;
using UnityEngine;
using UnityEngine.UI;
using View.Sound;

namespace View.Title
{
    public class CreateRoomTab : MonoBehaviour
    {
        public InputField password;
        public Button createButton;
        public int StageId = 1;

        [SerializeField]
        SoundView sound;

        private void OnDisable()
        {
            createButton.onClick.RemoveAllListeners();
        }

        public void OnStageButtonPressed(int stageId)
        {
            sound.PlaySE(AudioId.ButtonClick, 1f);
            StageId = stageId;
        }
    }
}
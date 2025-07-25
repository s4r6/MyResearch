using System.Collections.Generic;
using Presenter.Vote;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;
using View.Player;

namespace View.UI
{
    public class VoteView : MonoBehaviour, IVoteView
    {
        [SerializeField]
        GameObject CheckBoxPrefab;
        [SerializeField]
        Transform CheckBoxRoot;

        [SerializeField]
        Button YesButton;
        [SerializeField]
        Button NoButton;

        List<Image> images = new();

        VotePresenter presenter;

        public void Inject(VotePresenter presenter)
        {
            this.presenter = presenter;
        }

        void Start()
        {
            YesButton.onClick.AddListener(() => { presenter?.OnChoice(true); });
            NoButton.onClick.AddListener(() => { presenter?.OnChoice(false); });

            Hide();
        }

        public void Display()
        {
            this.gameObject.SetActive(true);
            PlayerView.cursorLocked = false;
            Cursor.lockState = CursorLockMode.None;
        }

        public void Hide()
        {
            PlayerView.cursorLocked = true;
            Cursor.lockState = CursorLockMode.Locked;
            gameObject.SetActive(false);
        }

        public void CreateCheckBox(int num)
        {
            for(int i = 0; i < num; i++)
            {
                var checkBox = Instantiate(CheckBoxPrefab, CheckBoxRoot);
                images.Add(checkBox.GetComponent<Image>());
            }
        }

        public void UpdateCheckBox(int yes, int no, int total)
        {
            // êÊì™Ç©ÇÁèáÇ…êFÇê›íË
            for (int i = 0; i < total; i++)
            {
                if (i < yes)
                {
                    images[i].color = Color.blue;
                }
                else if (i < yes + no)
                {
                    images[i].color = Color.red;
                }
                else
                {
                    images[i].color = Color.black;
                }
            }
        }
    }
}
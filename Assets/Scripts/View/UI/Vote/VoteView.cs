using System;
using System.Collections.Generic;
using Presenter.Vote;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
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
        /*[SerializeField]
        Button CancelButton;*/

        [SerializeField]
        TMP_Text Message;

        List<Image> images = new();

        VotePresenter presenter;

        public void Inject(VotePresenter presenter)
        {
            this.presenter = presenter;
        }


        Action<bool> OnSelect;
        void Start()
        {
            YesButton.onClick.AddListener(() => {
                OnSelect?.Invoke(true); 
                OnSelect = null;
            });

            NoButton.onClick.AddListener(() => { 
                OnSelect?.Invoke(false); 
                OnSelect = null;
            });

            Hide();
        }

        public void Display(Action<bool> onSelect)
        {
            OnSelect = onSelect;

            this.gameObject.SetActive(true);
            PlayerView.cursorLocked = false;
            Cursor.lockState = CursorLockMode.None;

            Message.text = "èIóπÇµÇ‹Ç∑Ç©ÅH";
            Message.gameObject.SetActive(true);
        }

        public void DisplayWaitingUI()
        {
            Message.gameObject.SetActive(false);
            YesButton.gameObject.SetActive(false);
            NoButton.gameObject.SetActive(false);

            //CancelButton.gameObject.SetActive(true);
        }

        public void Hide()
        {
            PlayerView.cursorLocked = true;
            Cursor.lockState = CursorLockMode.Locked;

            Message.gameObject.SetActive(true);
            YesButton.gameObject.SetActive(true);
            NoButton.gameObject.SetActive(true);

            //CancelButton.gameObject.SetActive(false);

            foreach(var img in images)
            {
                Destroy(img.gameObject);
            }
            images.Clear();
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
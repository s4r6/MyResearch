using System.Collections.Generic;
using NUnit.Framework;
using Presenter.Network;
using Presenter.Sound;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UseCase.Title;
using View.Sound;

namespace View.Title
{
    public class MultiPlayerView : MonoBehaviour, IMultiPlayerView
    {
        [SerializeField]
        Text PlayerName;

        [SerializeField]
        CreateRoomTab createRoomTab;
        [SerializeField]
        Button createRoomButton;

        [SerializeField]
        JoinRoomTab joinRoomTab;
        [SerializeField]
        Button joinRoomButton;

        [SerializeField]
        SoundView sound;

        RoomPresenter presenter;
        public void Injection(RoomPresenter presenter)
        {
            this.presenter = presenter;
        }

        private void Awake()
        {
            createRoomButton.onClick.AddListener(DisplayCreateRoomTab);
            joinRoomButton.onClick.AddListener(DisplayJoinRoomTab);
        }

        public void Activate()
        {
            gameObject.SetActive(true);
        }

        public void DeActivate()
        {
            gameObject.SetActive(false);
            createRoomTab.gameObject.SetActive(false);
            joinRoomTab.gameObject.SetActive(false);
        }

        public void SetPlayerName(string name)
        {
            PlayerName.text = name;
        }

        //CreateRoomボタンが押されたとき
        void DisplayCreateRoomTab()
        {
            Debug.Log("Tab表示");
            sound.PlaySE(AudioId.ButtonClick, 1f);
            createRoomTab.gameObject.SetActive(true);
            createRoomTab.createButton.onClick.AddListener(OnCreate);
        }

        //JoinRoomボタンが押されたとき
        void DisplayJoinRoomTab()
        {
            Debug.Log("JoinTab表示");
            sound.PlaySE(AudioId.ButtonClick, 1f);
            joinRoomTab.gameObject.SetActive(true);
            joinRoomTab.searchButton.onClick.AddListener(OnSearch);
        }

        //Createボタンが押されたとき
        void OnCreate()
        {
            Debug.Log("Create");
            sound.PlaySE(AudioId.ButtonClick, 1f);
            var roomId = createRoomTab.password.text;
            var stageId = createRoomTab.StageId;
            presenter.CreateRoom(roomId, PlayerName.text, stageId);
        }

        //Searchボタンが押されたとき
        void OnSearch()
        {
            Debug.Log("Search");
            sound.PlaySE(AudioId.ButtonClick, 1f);
            presenter.SearchRoom();
        }

        public void DisplayRoomList(List<(string, int)> rooms) => joinRoomTab.DisplayRoomList(rooms, (roomId) => { presenter.JoinRoom(roomId); });

        public void TransitionInGame(int stageId)
        {
            SceneManager.LoadScene("Stage" + stageId);
        }
    }
}
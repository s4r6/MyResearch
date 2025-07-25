using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace View.Title
{
    public class JoinRoomTab : MonoBehaviour
    {
        [SerializeField]
        public Button searchButton;
        [SerializeField]
        GameObject roomInfoPrefab;
        [SerializeField]
        Transform parentTransform;

        Action<string> SelectRoomEvent;

        private void OnDisable()
        {
            searchButton.onClick.RemoveAllListeners();
        }


        public void DisplayRoomList(List<(string, int)> roomDatas, Action<string> OnSelect)
        {
            SelectRoomEvent = OnSelect;

            foreach (var room in roomDatas)
            {
                var roomData = Instantiate(roomInfoPrefab, parentTransform);
                var setter = roomData.GetComponent<RoomInfoSetter>();
                setter?.SetInfo(room.Item1, room.Item2, (roomId) => OnSelectRoom(roomId));
            }
        }

        public void OnSelectRoom(string roomId)
        {
            SelectRoomEvent?.Invoke(roomId);
        }

        private void OnDestroy()
        {
            SelectRoomEvent = null;
        }
    }
}
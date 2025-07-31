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

        Dictionary<string, GameObject> rooms = new();

        private void OnDisable()
        {
            searchButton.onClick.RemoveAllListeners();
        }


        public void AddRoomList(List<(string, string, int)> roomDatas, Action<string> OnSelect)
        {
            SelectRoomEvent = OnSelect;

            foreach (var room in roomDatas)
            {
                var roomButton = Instantiate(roomInfoPrefab, parentTransform);
                var setter = roomButton.GetComponent<RoomInfoSetter>();
                rooms.Add(room.Item2, roomButton);
                setter?.SetInfo(room.Item1, room.Item2, room.Item3, (roomId) => OnSelectRoom(roomId));
            }
        }

        public void DestroyRoomList(List<string> names)
        {
            foreach(var name in names)
            {
                if (!rooms.TryGetValue(name, out var room))
                    continue;

                Destroy(room);
            }
        }

        public void UpdateRoomList(List<(string, string, int)> roomDatas)
        {
            foreach(var roomData in roomDatas )
            {
                if (!rooms.TryGetValue(roomData.Item1, out var roomButton))
                    continue;

                var setter = roomButton.GetComponent<RoomInfoSetter>();
                setter.SetInfo(roomData.Item1, roomData.Item2, roomData.Item3, (roomId) => OnSelectRoom(roomId));
            }
        }

        public void OnSelectRoom(string roomId)
        {
            SelectRoomEvent?.Invoke(roomId);
            SelectRoomEvent = null;
        }

        private void OnDestroy()
        {
            SelectRoomEvent = null;
        }
    }
}
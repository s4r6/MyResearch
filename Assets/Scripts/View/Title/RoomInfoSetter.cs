using System;
using UnityEngine;
using UnityEngine.UI;

public class RoomInfoSetter : MonoBehaviour
{
    [SerializeField]
    Text buttonText;
    [SerializeField]
    Button self;

    string roomId;
    string roomName;
    public Action<string> OnPressedEvent;
    public void SetInfo(string id, string name, int count, Action<string> onPressed)
    {
        OnPressedEvent = onPressed;
        roomName = name;
        roomId = id;
        buttonText.text = $"RoomId:{name}, PlayerNum:{count}";
    }

    public void OnPressed()
    {
        OnPressedEvent?.Invoke(roomId);
        OnPressedEvent = null;
    }

    public void OnDestroy()
    {
        OnPressedEvent = null;
    }

}

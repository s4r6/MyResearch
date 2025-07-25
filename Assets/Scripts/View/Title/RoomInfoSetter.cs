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
    public Action<string> OnPressedEvent;
    public void SetInfo(string id, int count, Action<string> onPressed)
    {
        OnPressedEvent = onPressed;
        roomId = id;
        buttonText.text = $"RoomId:{id}, PlayerNum:{count}";
    }

    public void OnPressed()
    {

        OnPressedEvent?.Invoke(roomId);
    }

    public void OnDestroy()
    {
        OnPressedEvent = null;
    }

}

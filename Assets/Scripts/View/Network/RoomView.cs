using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomView : MonoBehaviour
{
    public void DisplayRoom(string roomId, string hostId, bool IsSuccess)
    {
        Debug.Log($"RoomId:{roomId}\nHostId:{hostId}\nIsSuccess:{IsSuccess}");
    }

    public void TransitionInGameScene()
    {
        SceneManager.LoadScene("Stage1");
    }
}

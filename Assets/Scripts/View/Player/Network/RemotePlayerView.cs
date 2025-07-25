using UnityEngine;

namespace View.Network
{
    public class RemotePlayerView : MonoBehaviour
    {
        public void SetPosition(Vector3 position)
        {
            gameObject.transform.position = position;
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }
    }
}
using UnityEngine;

namespace View.Player
{
    public class RaycastController : MonoBehaviour
    {
        [SerializeField] private Camera playerCamera;
        [SerializeField] private float rayDistance = 20f;
        [SerializeField] private LayerMask raycastLayerMask;

        public string TryGetLookedObjectId()
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, raycastLayerMask))
            {
                Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.green);
                return hit.collider.gameObject.name;
            }
            Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.red);
            return string.Empty;
        }
    }
}
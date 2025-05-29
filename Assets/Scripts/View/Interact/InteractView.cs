using UnityEngine;

namespace View.Stage
{
    public class InteractView : MonoBehaviour
    {
        public void Interact(string objectId, string action)
        {
            var interactable = GameObject.Find(objectId).GetComponentInChildren<IInteractable>();
            if (interactable != null) 
            { 
                interactable.Interact(action);
            }
        }
    }
}
using UnityEngine;

namespace View.Stage
{
    public class Door : MonoBehaviour, IInteractable
    {
        private Transform _cashThisTransform;

        Vector3 startPosition = new Vector3(-90f, 180f, -180f);
        Vector3 afterPosition = new Vector3(-90f, 180f, -70f);


        // Start is called before the first frame update
        void Start()
        {
            _cashThisTransform = transform;
            _cashThisTransform.localEulerAngles = startPosition;
        }

        // Update is called once per frame

        public void Interact(string action)
        {
            if (action == "open")
            {
                _cashThisTransform.localEulerAngles = afterPosition;
            }
            else if(action == "close")
            {
                _cashThisTransform.localEulerAngles = startPosition;
            }
            
        }

    }
}
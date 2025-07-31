using Presenter.Sound;
using UnityEngine;
using View.Sound;

namespace View.Stage
{
    public class Door : MonoBehaviour, IInteractable
    {
        private Transform _cashThisTransform;

        Vector3 startPosition = new Vector3(-90f, 180f, -180f);
        Vector3 afterPosition = new Vector3(-90f, 180f, -70f);

        [SerializeField]
        SoundView sound;

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
                sound.PlaySE(AudioId.DoorOpen, 1f);
                _cashThisTransform.localEulerAngles = afterPosition;
            }
            else if(action == "close")
            {
                sound.PlaySE(AudioId.DoorClose, 1f);
                _cashThisTransform.localEulerAngles = startPosition;
            }
            
        }

    }
}
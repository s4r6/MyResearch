using UnityEngine;
using UnityEngine.InputSystem;
using Presenter.Tutorial;

namespace View.Tutorial
{
    public class PlayerInputActionMapController : MonoBehaviour, IActionMapController
    {
        [SerializeField] private PlayerInput playerInput;

        public string CurrentActionMapName => playerInput.currentActionMap.name;

        public void SwitchTo(string actionMapName)
        {
            playerInput.SwitchCurrentActionMap(actionMapName);
        }
    }
}
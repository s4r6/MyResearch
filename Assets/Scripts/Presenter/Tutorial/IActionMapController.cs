using UnityEngine;

namespace Presenter.Tutorial
{
    public interface IActionMapController
    {
        string CurrentActionMapName { get; }
        void SwitchTo(string actionMapName);
    }
}
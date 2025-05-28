using UnityEngine;
using View.Player;

namespace Presenter.Player
{
    public class PlayerCarryPresenter
    {
        private readonly IPlayerCarryView view;

        public PlayerCarryPresenter(IPlayerCarryView view)
        {
            this.view = view;

        }

        public void HoldObject(string objectId)
        {
            if (string.IsNullOrEmpty(objectId))
            {

                return;
            }
            

            view.HoldObject(objectId);

        }

        public void ReleaseObject(string objectId)
        {

            if (string.IsNullOrEmpty(objectId))
            {

                return;
            }
            
            view.ReleaseObject(objectId);

        }
    }
} 
using Domain.Network;
using UnityEngine;
using UseCase.Network;
using View.Network;

namespace Presenter.Network
{
    public class RemotePlayerPresenter
    {
        RemotePlayerView view;

        public RemotePlayerPresenter(RemotePlayerView view)
        {
            this.view = view;
        }

        public void Move(Position newPos)
        {
            var position = newPos.ToVector3();
            view.SetPosition(position);
        }

        public void Destroy()
        {
            view.Destroy();
        }
    }
}
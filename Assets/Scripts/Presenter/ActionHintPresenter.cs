using System.Collections.Generic;
using UnityEngine;
using UseCase.Network.DTO;
using UseCase.Player;

namespace Presenter.Player
{
    public interface IActionHintView
    {
        void ShowHintList(IEnumerable<ActionHint> hints);
    }
    public class ActionHintPresenter : IActionHintPresenter
    {
        private readonly IActionHintView _view;

        public ActionHintPresenter(IActionHintView view)
        {
            _view = view;
        }

        public void ShowAvailableActions(IEnumerable<ActionHint> hints)
        {
            _view.ShowHintList(hints);
        }
    }
}
using System;
using UnityEngine;

namespace Presenter.Tutorial
{
    public interface ITutorialInput
    {
        /// Tutorial—p‚ÌuNextv“ü—Í‚ª—ˆ‚½‚Æ‚«
        event Action NextRequested;
    }
}
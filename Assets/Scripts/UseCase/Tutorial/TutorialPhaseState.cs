using Domain.Tutorial;
using UnityEngine;

namespace UseCase.Tutorial
{
    public class TutorialPhaseState
    {
        TutorialPhase current;

        public bool IsAtOrAfter(TutorialPhase phase)
            => current >= phase;

        public void AdvanceTo(TutorialPhase phase)
            => current = phase;
    }
}
using Domain.Tutorial;
using UnityEngine;

public class TutorialRestriction : IGameRestriction
{
    TutorialPhaseState phase;

    public TutorialRestriction(TutorialPhaseState phase)
    {
        this.phase = phase;
    }

    public bool CanInspect()
        => phase.IsAtOrAfter(TutorialPhase.Phase2_InspectPC1);

    public bool CanSelectRisk()
        => phase.IsAtOrAfter(TutorialPhase.Phase3_RiskExplanation);

    public bool CanEndGame()
        => phase.IsAtOrAfter(TutorialPhase.Phase11_EndGameInstruction);
}
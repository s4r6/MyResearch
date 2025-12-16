using UnityEngine;

namespace Domain.Tutorial
{
    public enum TutorialPhase
    {
        None = 0,

        Phase0_Intro,
        Phase1_ReadDocument,
        Phase2_InspectPC1,
        Phase3_RiskExplanation,
        Phase4_OpenActionList,
        Phase5_SelectAction,
        Phase6_DescribeRiskAssessment,
        Phase7_InspectMemo,
        Phase8_PickupMemo,
        Phase9_UseShredder,
        Phase10_FreeAssessment,
        Phase11_EndGameInstruction,
        Phase12_Result,

        Completed
    }

    public class TutorialPhaseState
    {
        TutorialPhase currentPhase = TutorialPhase.None;

        public TutorialPhase Current => currentPhase;

        public void AdvanceTo(TutorialPhase next)
        {
            if (next < currentPhase)
                return; // 後戻り防止（必要なら例外）

            currentPhase = next;
        }

        public bool IsAtOrAfter(TutorialPhase phase)
        {
            return currentPhase >= phase;
        }
    }
}
using Domain.Action;
using Domain.Stage;
using UnityEngine;

namespace UseCase.Stage
{
    public class StageSystemUseCase
    {
        StageEntity stage;
        public StageSystemUseCase(StageEntity stage)
        {
            this.stage = stage;
        }

        public void OnExecuteAction(ActionEntity action)
        {
            /*stage.OnExecuteAction(action);
            Debug.Log("ActionPointAmount;" + stage.GetActionPoint());
            Debug.Log("RiskAmount:" + stage.GetRiskAmount());
            */
        }

        
    }
}

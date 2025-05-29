using Domain.Action;
using Domain.Stage;
using UnityEngine;
using UseCase.Player;
using View.UI;

namespace UseCase.Stage
{
    public class StageSystemUseCase
    {
        StageEntity stage;
        ResultView view;
        public StageSystemUseCase(StageEntity stage, ResultView view)
        {
            this.stage = stage;
            this.view = view;
        }

        public void OnExecuteAction(ActionHistory history)
        {
            stage.OnExecuteAction(history);
        }

        public void OnExitStage()
        {
            Debug.Log("ActionPointAmount;" + stage.GetActionPoint());
            Debug.Log("RiskAmount:" + stage.GetRiskAmount());
            view.ShowResultWindow(stage.histories);
        }
    }
}

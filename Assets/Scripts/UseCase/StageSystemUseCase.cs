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

        public void OnExitStage()
        {
            view.ShowResultWindow(stage.CreateSurmmary());
        }
    }
}

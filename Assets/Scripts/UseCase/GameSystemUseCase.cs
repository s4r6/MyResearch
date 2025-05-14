using UnityEngine;
using UseCase.Player;
using UniRx;
using UseCase.Stage;
using System;

namespace UseCase.GameSystem
{
    public class GameSystemUseCase : IDisposable
    {
        PlayerSystemUseCase player;
        StageSystemUseCase stage;

        CompositeDisposable disposables = new CompositeDisposable();

        public GameSystemUseCase(PlayerSystemUseCase player, StageSystemUseCase stage)
        {
            this.player = player;
            this.stage = stage;
        }

        public void StartGame()
        {
            player.StartGame();
            player.OnActionExecute
                .Subscribe(x =>
                {
                    stage.OnExecuteAction(x);
                }).AddTo(disposables);

            player.OnExitPointInspected
                .Subscribe(x => 
                { 
                    EndGame();
                }).AddTo(disposables);
        }

        public void EndGame()
        {
            player.EndGame();
            Debug.Log("ÉQÅ[ÉÄèIóπ:");
            Dispose();
        }

        public void Dispose()
        {
            disposables.Dispose();
        }
    }
}

using UnityEngine;
using UseCase.Player;
using UniRx;
using UseCase.Stage;
using System;
using Domain.Game;

namespace UseCase.GameSystem
{
    public class GameSystemUseCase : IDisposable
    {
        PlayerSystemUseCase player;
        StageSystemUseCase stage;
        GameStateManager state;

        CompositeDisposable disposables = new CompositeDisposable();

        public GameSystemUseCase(PlayerSystemUseCase player, StageSystemUseCase stage, GameStateManager state)
        {
            this.player = player;
            this.stage = stage;
            this.state = state;
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
            Debug.Log("ÉQÅ[ÉÄèIóπ");
            state.Set(GamePhase.Result);
            stage.OnExitStage();
            Dispose();
        }

        public void Dispose()
        {
            disposables.Dispose();
        }
    }
}

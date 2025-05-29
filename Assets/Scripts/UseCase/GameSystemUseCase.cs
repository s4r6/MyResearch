using UnityEngine;
using UseCase.Player;
using UniRx;
using UseCase.Stage;
using System;
using Domain.Game;
using View.Player;
using UseCase.Game;

namespace UseCase.GameSystem
{
    public class GameSystemUseCase : IDisposable
    {
        PlayerSystemUseCase player;
        StageSystemUseCase stage;
        GameStateManager state;
        DocumentUseCase document;
        InputController input;

        CompositeDisposable disposables = new CompositeDisposable();

        public GameSystemUseCase(PlayerSystemUseCase player, StageSystemUseCase stage, GameStateManager state, DocumentUseCase document, InputController input)
        {
            this.player = player;
            this.stage = stage;
            this.state = state;
            this.document = document;
            this.input = input;
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

            input.OnDocumentButtonPressed
                .Subscribe(_ =>
                {
                    Debug.Log("Document");
                    document.OpenDocument(() =>
                    {
                        state.Set(GamePhase.Moving);
                    });
                    
                    state.Set(GamePhase.Document);
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

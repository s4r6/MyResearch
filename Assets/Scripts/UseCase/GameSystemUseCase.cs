using UnityEngine;
using UseCase.Player;
using UniRx;
using UseCase.Stage;
using System;
using Domain.Game;
using View.Player;
using UseCase.Game;
using Presenter.Tutorial;
using Cysharp.Threading.Tasks;
using View.UI;

namespace UseCase.GameSystem
{
    public enum TutorialStepId
    {
        Explain_Game,
        Explain_Document,
    }

    public class GameSystemUseCase : IDisposable
    {
        ResultView view;
        InputController input;

        GameEntity game;

        CompositeDisposable disposables = new CompositeDisposable();
        public Subject<Unit> OnStartGame = new();
        public Subject<Unit> OnEndGame = new();

        public GameSystemUseCase(ResultView view, GameEntity entity, InputController input)
        {
            this.view = view;
            this.game = entity;
            this.input = input;
        }

        public async UniTask StartGame()
        {
            input.OnFinishButtonPressed
                .Subscribe(x => 
                { 
                    EndGame();
                    OnEndGame.OnNext(default);
                }).AddTo(disposables);

            OnStartGame.OnNext(default);
        }



        public void EndGame()
        {
            var result = game.EndGame();
            view.ShowResultWindow(result.surmmary);
        }

        public void Dispose()
        {
            disposables.Dispose();
        }
    }
}

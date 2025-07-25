using System;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;
using UseCase.GameSystem;
using View.UI;

namespace Presenter.Vote
{
    public interface IVoteView
    {
        void Display();
        void Hide();
        void CreateCheckBox(int num);
        void UpdateCheckBox(int YesNum, int NoNum, int Total);
    }

    public class VotePresenter : IDisposable
    {
        VoteUseCase voteUseCase;
        RemoteGameSystemUseCase gameUseCase;

        IVoteView view;

        bool IsDisplayed = false;

        CompositeDisposable _disposables = new CompositeDisposable();
        public VotePresenter(VoteUseCase voteUseCase, RemoteGameSystemUseCase gameUseCase, IVoteView view)
        {
            this.voteUseCase = voteUseCase;
            this.gameUseCase = gameUseCase;
            this.view = view;

            Bind();
        }

        public void OnChoice(bool isYes)
        {
            Debug.Log(isYes);
            if (isYes)
            {
                voteUseCase.SendVoteChoice(VoteChoice.Yes).Forget();
            }
            else
            {
                voteUseCase.SendVoteChoice(VoteChoice.No).Forget();
            }
        }

        void Bind()
        {
            voteUseCase.OnVoteStarted
                .Subscribe(x =>
                {
                    Debug.Log(view);
                    view.CreateCheckBox(x);
                    view.Display();
                    IsDisplayed = true;
                }).AddTo(_disposables);

            voteUseCase.OnVoteUpdated
                .Subscribe(x =>
                {
                    var YesNum = x.Yes;
                    var NoNum = x.No;
                    if(!IsDisplayed)
                    {
                        view.CreateCheckBox(x.Total);
                        view.Display();
                        IsDisplayed = true;
                    }
                    view.UpdateCheckBox(YesNum, NoNum, x.Total);
                }).AddTo(_disposables);

            voteUseCase.OnVotePassed
                .Subscribe(_ =>
                {
                    view.Hide();
                    gameUseCase.EndGame();
                }).AddTo(_disposables);

            voteUseCase.OnVoteFailed
                .Subscribe(_ =>
                {
                    view.Hide();
                    gameUseCase.FailedVote();
                }).AddTo (_disposables);
        }

        public void Dispose()
        {
           _disposables?.Dispose();
        }
    }
}
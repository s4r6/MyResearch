using System;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;
using UseCase.GameSystem;
using View.UI;

namespace Presenter.Vote
{
    public interface IVoteView
    {
        void Display(Action<bool> isYes);
        void Hide();
        void DisplayWaitingUI();
        void CreateCheckBox(int num);
        void UpdateCheckBox(int YesNum, int NoNum, int Total);
    }

    public class VotePresenter : IVotePresenter, IDisposable
    {
        //VoteUseCase voteUseCase;

        IVoteView view;

        bool IsDisplayed = false;

        CompositeDisposable _disposables = new CompositeDisposable();
        public VotePresenter(IVoteView view)
        {
            this.view = view;
            Bind();
        }

        /*public void OnChoice(bool isYes)
        {
            Debug.Log(isYes);
            if (isYes)
            {
                OnSelect?.Invoke(isYes);
                view.DisplayWaitingUI();
                //voteUseCase.SendVoteChoice(VoteChoice.Yes).Forget();
            }
            else
            {
                OnSelect?.Invoke(isYes);
                view.Hide();
                //voteUseCase.SendVoteChoice(VoteChoice.No).Forget();
            }
        }*/

        Action<bool> OnComplete;
        public void StartVote(int playerNum ,Action<bool> onComplete)
        {
            OnComplete = onComplete;

            Debug.Log(view);
            view.Display((isYes) =>
            {
                if (isYes)
                {
                    view.CreateCheckBox(playerNum);
                    OnComplete?.Invoke(isYes);
                    OnComplete = null;
                    view.DisplayWaitingUI();
                    IsDisplayed = true;
                    //voteUseCase.SendVoteChoice(VoteChoice.Yes).Forget();
                }
                else
                {
                    OnComplete?.Invoke(isYes);
                    OnComplete = null;
                    view.Hide();
                    IsDisplayed = false;
                    //voteUseCase.SendVoteChoice(VoteChoice.No).Forget();
                }
            });
            IsDisplayed = true;
        }

        Action<VoteChoice> OnSelect;
        public void UpdateVote(VoteProgress progress, Action<VoteChoice> onSelect)
        {
            OnSelect = onSelect;

            var YesNum = progress.Yes;
            var NoNum = progress.No;
            if (!IsDisplayed)
            {
                view.Display((isYes) =>
                {
                    view.CreateCheckBox(progress.Total);
                    if (isYes)
                    {
                        OnSelect?.Invoke(VoteChoice.Yes);
                        OnSelect = null;
                        view.DisplayWaitingUI();
                        IsDisplayed = true;
                    }
                    else
                    {
                        OnSelect?.Invoke(VoteChoice.No);
                        OnSelect = null;
                        view.DisplayWaitingUI();
                        IsDisplayed = true;
                    }

                    
                });
                IsDisplayed = true;
            }
            else
            {
                view.UpdateCheckBox(YesNum, NoNum, progress.Total);
            }
        }

        public void OnVotePassed()
        {
            view.Hide();
            IsDisplayed = false;
        }

        public void OnVoteFailed()
        {
            view.Hide();
            IsDisplayed = false;
        }

        void Bind()
        {
            /*voteUseCase.OnVoteStarted
                .Subscribe(x =>
                {
                    Debug.Log(view);
                    view.CreateCheckBox(x);
                    view.Display();
                    IsDisplayed = true;
                }).AddTo(_disposables);*/

            /*voteUseCase.OnVoteUpdated
                .Subscribe(x =>
                {
                    
                }).AddTo(_disposables);*/

           /* voteUseCase.OnVotePassed
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
                }).AddTo (_disposables);*/
        }

        public void Dispose()
        {
           _disposables?.Dispose();
        }
    }
}
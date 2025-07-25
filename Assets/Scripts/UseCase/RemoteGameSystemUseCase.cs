using Domain.Game;
using UniRx;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UseCase.Game;
using UseCase.Network;
using UseCase.Player;
using UseCase.Stage;
using View.Player;

namespace UseCase.GameSystem
{
    public class RemoteGameSystemUseCase
    {
        PlayerSystemUseCase player;
        StageSystemUseCase stage;
        GameStateManager state;
        DocumentUseCase document;
        InputController input;
        VoteUseCase vote;

        IPacketSender sender;
        IPacketReceiver receiver;
        ISessionRepository session;
        
        CompositeDisposable disposables = new CompositeDisposable();

        public RemoteGameSystemUseCase(PlayerSystemUseCase player, StageSystemUseCase stage, GameStateManager state, DocumentUseCase document, InputController input, VoteUseCase vote, ISessionRepository session, IPacketSender sender, IPacketReceiver receiver)
        {
            this.player = player;
            this.stage = stage;
            this.state = state;
            this.document = document;
            this.input = input;
            this.vote = vote;
            this.sender = sender;
            this.session = session;
            this.receiver = receiver;
        }

        public void StartGame()
        {
            player.StartGame();

            player.OnExitPointInspected
                .Subscribe(x =>
                {
                    OnVoteEvent();
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

        public void OnVoteEvent()
        {
            state.Set(GamePhase.Vote);
            vote.StartVote();
        }

        public void EndGame()
        {
            player.EndGame();
            Debug.Log("ÉQÅ[ÉÄèIóπ");
            state.Set(GamePhase.Result);
            stage.OnExitStage();
            Dispose();
        }

        public void FailedVote()
        {
            state.Set(GamePhase.Moving);
        }

        public void Dispose()
        {
            disposables.Dispose();
        }
    }
}
using UniRx;
using UnityEngine;

namespace Domain.Game
{
    public class GameStateManager
    {
        public Subject<GamePhase> OnChangeState = new();
        public GameState Current { get; private set; } = new GameState(GamePhase.Moving);

        public void Set(GamePhase phase)
        {
            Current = Current.WithPhase(phase);
            OnChangeState.OnNext(phase);
        }
    }
}
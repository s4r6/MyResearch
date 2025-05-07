using UnityEngine;

namespace Domain.Game
{
    public class GameStateManager
    {
        public GameState Current { get; private set; } = new GameState(GamePhase.Moving);

        public void Set(GamePhase phase)
        {
            Debug.Log($"Change State To :{phase}");
            Current = Current.WithPhase(phase);
        }
    }
}
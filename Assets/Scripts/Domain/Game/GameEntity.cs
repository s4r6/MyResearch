using Domain.Game;
using Domain.Stage;
using UnityEngine;

namespace Domain.Game
{
    public struct EndGameResult
    {
        public SurmmaryDTO surmmary;
    }

    public class GameEntity
    {
        public StageEntity stage;
        public GameStateManager state;

        public GameEntity(StageEntity stage, GameStateManager state)
        {
            this.stage = stage;
            this.state = state;
        }

        public EndGameResult EndGame()
        {
            if (!state.Current.IsMoving) throw new System.InvalidOperationException();

            state.Set(GamePhase.Result);
            return new EndGameResult
            {
                surmmary = stage.CreateSurmmary(),
            };
        }
    }
}

using UnityEngine;
using UseCase.Player;
using UseCase.Stage;

namespace UseCase.GameSystem
{
    public class GameSystemUseCase
    {
        PlayerSystemUseCase player;
        StageSystemUseCase stage;

        public GameSystemUseCase(PlayerSystemUseCase player, StageSystemUseCase stage)
        {
            this.player = player;
            this.stage = stage;
        }

        public void StartGame()
        {
            //player.StartGame();
        }

        public void EndGame()
        {
            //player.EndGame();
        }
    }
}

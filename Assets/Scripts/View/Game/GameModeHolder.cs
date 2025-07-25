using UnityEngine;
using UseCase.Title;

namespace Infrastructure.Game
{

    public class GameModeHolder : MonoBehaviour
    {
        public GameMode CurrentMode { get; private set; } = GameMode.ModeSelect;

        public void SetMode(GameMode mode) 
        {
            CurrentMode = mode;
        }

        void Awake()
        {
            DontDestroyOnLoad(gameObject);    
        }
    }
}
using UnityEngine;

namespace Infrastructure.Game
{
    public enum GameMode
    {
        None,
        Solo,
        Multi
    }
    public class GameModeHolder : MonoBehaviour
    {
        public GameMode CurrentMode {  get; private set; }

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
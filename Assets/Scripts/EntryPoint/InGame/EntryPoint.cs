using Domain.Player;
using UnityEngine;
using UseCase.Player;
using View.Player;
using View.UI;
using Domain.Game;
using Presenter.Player;
using Infrastructure.Repository;
using UseCase.GameSystem;
using UseCase.Stage;
using Infrastructure.Factory;
using Domain.Stage;
using View.Stage;
using UseCase.Game;
using Infrastructure.Game;
using Infrastructure.Network;
using UseCase.Title;

namespace EntryPoint
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField]
        InGameEntryPoint_Local local;
        [SerializeField]
        InGameEntryPoint_Network network;

        void Awake()
        {
            var gameMode = FindFirstObjectByType<GameModeHolder>();
            

            if(gameMode?.CurrentMode == GameMode.Solo)
            {
                local.Entry();
            }
            else if(gameMode?.CurrentMode == GameMode.Multi)
            {
                network.Entry();
            }
            else
            {
                local.Entry();
            }
        }
    }
}


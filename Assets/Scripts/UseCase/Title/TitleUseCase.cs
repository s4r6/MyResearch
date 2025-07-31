using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Domain.Network;
using Infrastracture.Network;
using Infrastracture.Repository;
using Infrastructure.Game;
using Presenter.Network;
using Presenter.Sound;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UseCase.Network;
using View.Title;

namespace UseCase.Title
{
    public enum GameMode
    {
        None,
        ModeSelect,
        Solo,
        Multi
    }

    public interface IModeSelectView
    {
        void WarningInputName();
        void Activate();
        void DeActivate();
    }

    public interface ISinglePlayerView
    {
        void Activate();
        void DeActivate();
        void SetPlayerName(string playerName);
        void TransitionInGame(int stageId);
    }

    public class TitleUseCase 
    {
        GameModeHolder gameMode;
        IModeSelectView modeSelect;
        ISinglePlayerView single;
        IMultiPlayerView multi;

        IWebSocketService socket;

        string cashPlayerName = string.Empty;

        public TitleUseCase(GameModeHolder gameMode, IModeSelectView modeSelect, ISinglePlayerView single, IMultiPlayerView multi, IWebSocketService socket)
        {
            this.gameMode = gameMode;
            this.modeSelect = modeSelect;
            this.single = single;
            this.multi = multi;

            this.socket = socket;
        }


        public async UniTask ChangeGameMode(GameMode mode)
        {
            if(gameMode.CurrentMode == GameMode.ModeSelect && cashPlayerName == string.Empty)
            {
                Debug.Log("Œx");
                modeSelect.WarningInputName();
                return;
            }

            CurrentModeViewDeActivate(gameMode.CurrentMode);

            gameMode.SetMode(mode);
            
            await CurrentModeViewActivate(gameMode.CurrentMode);
        }

        public void OnPlayerNameInputed(string name)
        {
           cashPlayerName = name;
        }

        public void StartGame(int id)
        {
            if (gameMode.CurrentMode == GameMode.Solo) 
            {
                single.TransitionInGame(id);
            }
            else
            {
                multi.TransitionInGame(id);
            }
        }

        async Task CurrentModeViewDeActivate(GameMode mode)
        {
            switch(mode)
            {
                case GameMode.ModeSelect:
                    modeSelect.DeActivate();
                    break;
                case GameMode.Solo:
                    single.DeActivate();
                    break;
                case GameMode.Multi:
                    if(socket.IsConnected)
                    {
                        await socket.Close();
                    }
                    multi.DeActivate();
                    break;
                default:
                    break;
            }
        }

        async UniTask CurrentModeViewActivate(GameMode mode)
        {
            switch (mode)
            {
                case GameMode.ModeSelect:
                    modeSelect.Activate();
                    break;
                case GameMode.Solo:
                    single.SetPlayerName(cashPlayerName);
                    single.Activate();
                    break;
                case GameMode.Multi:
                    multi.SetPlayerName(cashPlayerName);
                    if (!socket.IsConnected)
                    {
                        await socket.Connect();
                        Debug.Log("ConnectI—¹");
                        await UniTask.WaitUntil(() => socket.IsConnected);
                        Debug.Log("Ú‘±");
                    }
                    multi.Activate();
                    break;
                default:
                    break;
            }
        }
    }
}
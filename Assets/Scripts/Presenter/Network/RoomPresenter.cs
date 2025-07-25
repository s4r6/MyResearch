using System.Collections.Generic;
using System.Diagnostics;
using Cysharp.Threading.Tasks;
using UseCase.Network;
using UseCase.Network.DTO;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Presenter.Network
{
    public interface IMultiPlayerView
    {
        void Activate();
        void DeActivate();
        void TransitionInGame(int stageId);

        void SetPlayerName(string name);
        void DisplayRoomList(List<(string, int)> rooms);
    }

    public class RoomPresenter
    {
        RoomUseCase usecase; 
        IMultiPlayerView view;

        public RoomPresenter(IMultiPlayerView view, RoomUseCase usecase) 
        { 
            this.view = view;
            this.usecase = usecase;
        }

        public void CreateRoom(string roomId, string name, int stageId = 1)
        {
            var dto = new CreateRoomInputData
            {
                RoomId = roomId,
                PlayerName = name,
                StageId = stageId,
            };

            usecase.Create(dto, (result) => OnCompleteCreate(result)).Forget();
        }

        public void SearchRoom()
        {
            usecase.Search((result) => OnCompleteSearch(result)).Forget();
        }

        public void JoinRoom(string roomId)
        {
            var dto = new JoinRoomInputData
            {
                RoomId = roomId
            };

            usecase.Join(dto, (result) => OnCompleteJoin(result)).Forget();
        }

        public void OnCompleteCreate(CreateRoomOutputData output)
        {
            view.TransitionInGame(output.StageId);
        }

        public void OnCompleteSearch(SearchRoomOutputData output)
        {
            view.DisplayRoomList(output.RoomDatas);
        }

        public void OnCompleteJoin(JoinRoomOutputData output)
        {
            view.TransitionInGame(output.StageId);
        }
    }
}

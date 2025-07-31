using System.Collections.Generic;
using System.Diagnostics;
using Cysharp.Threading.Tasks;
using System.Linq;
using UseCase.Network;
using UseCase.Network.DTO;
using UnityEngine;
using Debug = UnityEngine.Debug;
using UseCase.Title;

namespace Presenter.Network
{
    public interface IMultiPlayerView
    {
        void Activate();
        void DeActivate();
        void TransitionInGame(int stageId);

        void SetPlayerName(string name);
        void AddRoomList(List<(string, string, int)> roomDatas);
        void DestroyRoomList(List<string> roomNames);
        void UpdateRoomList(List<(string, string, int)> roomDatas); 
    }

    public class RoomPresenter
    {
        RoomUseCase usecase;
        TitleUseCase titleUseCase;
        IMultiPlayerView view;

        public RoomPresenter(IMultiPlayerView view, RoomUseCase usecase, TitleUseCase titleUseCase) 
        { 
            this.view = view;
            this.usecase = usecase;
            this.titleUseCase = titleUseCase;
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

        public void OnBack()
        {
            titleUseCase.ChangeGameMode(GameMode.ModeSelect);
        }

        public void OnCompleteSearch(SearchRoomOutputData output)
        {
            var removedRoomNames = output.Removed.Select(r =>
            {
                return r.Name;
            }).ToList();
            view.DestroyRoomList(removedRoomNames);

            var updatedRoomDatas = output.Updated.Select(r => 
            {
                return (r.Id, r.Name, r.Players.Count);
            }).ToList();
            view.UpdateRoomList(updatedRoomDatas);

            var addedRoomDatas = output.Added.Select(r =>
            {
                return (r.Id, r.Name, r.Players.Count);
            }).ToList();
            view.AddRoomList(addedRoomDatas);
        }

        public void OnCompleteJoin(JoinRoomOutputData output)
        {
            view.TransitionInGame(output.StageId);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Domain.Network;
using Domain.Stage.Object;
using Infrastructure.Game;
using Infrastructure.Network;
using Infrastructure.Repository;
using UseCase.Network;
using UseCase.Network.DTO;

namespace Presenter.Network
{
    public class RoomPresenter
    {
        RoomUseCase usecase; 
        RoomView view;

        SessionHolder holder;
        GameModeHolder gameMode;

        public RoomPresenter(RoomView view, RoomUseCase usecase, SessionHolder holder) 
        { 
            this.view = view;
            this.usecase = usecase;

            this.holder = holder;
        }

        public void CreateRoom(string roomId, string playerName)
        {
            var dto = new CreateRoomInputData
            {
                RoomId = roomId,
                PlayerName = playerName,
                StageId = 1
            };

            usecase.Create(dto, (result, room) => OnCompleteCreate(result, room)).Forget();
        }

        public void OnCompleteCreate(CreateRoomOutputData output, RoomSession room)
        {
            view.DisplayRoom(output.RoomId, output.ConnectionId, output.Success);

            holder.SetSession(room);

            view.TransitionInGameScene();
        }
    }
}

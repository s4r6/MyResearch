using Domain.Network;
using Infrastracture.Network;
using Infrastracture.Repository;
using Infrastructure.Game;
using Infrastructure.Network;
using Infrastructure.Repository;
using Presenter.Network;
using TMPro;
using UnityEngine;
using UseCase.Network;
using UseCase.Title;
using View.Title;

public class TitleEntryPoint : MonoBehaviour
{
    [SerializeField]
    SinglePlayerView single;
    [SerializeField]
    ModeSelectView modeSelect;
    [SerializeField]
    MultiPlayerView multi;

    [SerializeField]
    GameModeHolder gameModeHolder;
    [SerializeField]
    SessionHolder sessionHolder;
    [SerializeField]
    ObjectRepositoryHolder objectRepositoryHolder;
    [SerializeField]
    StageRepositoryHolder stageRepositoryHolder;
    [SerializeField]
    ReceiverHolder receiverHolder;

    [SerializeField]
    NativeWebSocketService socket;
    private void Awake()
    {
        
        var sessionRepository = new RoomRepository(socket);
        var remoteStageRepository = new RemoteStageRepository(socket);
        var remoteObjectRepository = new RemoteObjectRepository(socket);
        var receiver = new WebSocketReceiver(socket);

        objectRepositoryHolder.SetRepository(remoteObjectRepository);
        stageRepositoryHolder.SetRepository(remoteStageRepository);
        sessionHolder.SetSession(sessionRepository);
        receiverHolder.SetReceiver(receiver);   

        var titleUseCase = new TitleUseCase(gameModeHolder, modeSelect, single, multi, socket);
        var roomUseCase = new RoomUseCase(sessionRepository, remoteObjectRepository, remoteStageRepository, receiver);

        var presenter = new RoomPresenter(multi, roomUseCase);

        modeSelect.Injection(titleUseCase);
        single.Injection(titleUseCase);
        multi.Injection(presenter);

        single.DeActivate();
        multi.DeActivate();
    }
}

using Cysharp.Threading.Tasks;
using Infrastracture.Network;
using Infrastracture.Repository;
using Infrastructure.Game;
using Infrastructure.Network;
using Infrastructure.Repository;
using Presenter.Network;
using UnityEngine;
using UseCase.Network;
using UseCase.Title;

public class NetworkTitleEntryPoint : MonoBehaviour
{
    [SerializeField]
    NativeWebSocketService socket;
    [SerializeField]
    RoomView roomView;
    [SerializeField]
    GameModeHolder gameMode;
    [SerializeField]
    SessionHolder sessionHolder;
    [SerializeField]
    ObjectRepositoryHolder objectRepositoryHolder;
    [SerializeField]
    StageRepositoryHolder stageRepositoryHolder;


    RoomPresenter presenter;

    async void Awake()
    {
        gameMode.SetMode(GameMode.Multi);

        var repository = new RoomRepository(socket);
        var remoteStageRepository = new RemoteStageRepository(socket);
        var remoteObjectRepository = new RemoteObjectRepository(socket);
       
        objectRepositoryHolder.SetRepository(remoteObjectRepository);
        stageRepositoryHolder.SetRepository(remoteStageRepository);
        sessionHolder.SetSession(repository);

        var receiver = new WebSocketReceiver(socket);
        
        var usecase = new RoomUseCase(repository, remoteObjectRepository, remoteStageRepository, receiver);
        //presenter = new RoomPresenter(usecase);        
    }

    async void Start()
    {
        await UniTask.WaitUntil(() => socket.ConnectionState == NativeWebSocket.WebSocketState.Open);
        Debug.Log("�ڑ�����");

        //presenter.CreateRoom("testRoom", "testPlayer");
    }
}

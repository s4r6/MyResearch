using Cysharp.Threading.Tasks;
using Infrastracture.Network;
using Infrastracture.Repository;
using Infrastructure.Game;
using Infrastructure.Network;
using Infrastructure.Repository;
using Presenter.Network;
using UnityEngine;
using UseCase.Network;

public class NetworkEntryPoint : MonoBehaviour
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
    ObjectRepositoryHolder repositoryHolder;


    RoomPresenter presenter;

    async void Awake()
    {
        gameMode.SetMode(GameMode.Multi);

        var repository = new RoomRepository(socket);
        var remoteObjectRepository = new RemoteObjectRepository(socket);

        repositoryHolder.SetRepository(remoteObjectRepository);

        var usecase = new RoomUseCase(repository);
        presenter = new RoomPresenter(roomView, usecase, sessionHolder);        
    }

    async void Start()
    {
        await UniTask.WaitUntil(() => socket.ConnectionState == NativeWebSocket.WebSocketState.Open);
        Debug.Log("Ú‘±Š®—¹");

        presenter.CreateRoom("testRoom", "testPlayer");
    }
}

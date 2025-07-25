using Domain.Network;
using Presenter.Network;
using UnityEngine;
using UseCase.Network;

namespace View.Network
{
    public class RemotePlayerViewFactory : MonoBehaviour, IRemotePlayerViewFactory
    {
        [SerializeField]
        GameObject spawnPos;
        [SerializeField]
        GameObject remotePlayerPrefab;

        public RemotePlayerPresenter SpawnRemotePlayer(IRemotePlayerRepository repository, string id)
        {
            //Entityê∂ê¨
            Position position = Position.FromVector3(spawnPos.transform.position);
            var entity = new RemotePlayerEntity(id, position);
            repository.Add(entity.Id, entity);

            var view = Instantiate(remotePlayerPrefab, spawnPos.transform.position, Quaternion.identity).GetComponent<RemotePlayerView>();

            var presenter = new RemotePlayerPresenter(view);

            return presenter;
        }
    }
}
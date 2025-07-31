using Infrastracture.Repository;
using UnityEngine;

namespace Infrastracture.Network
{
    public class StageRepositoryHolder : MonoBehaviour
    {
        public RemoteStageRepository repository {  get; private set; }
        public void SetRepository(RemoteStageRepository repository)
        {
            this.repository = repository;
        }

        void Awake()
        {
            DontDestroyOnLoad(this);
        }
    }
}
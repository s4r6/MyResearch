using Infrastructure.Repository;
using UnityEngine;

namespace Infrastructure.Network
{
    public class ObjectRepositoryHolder : MonoBehaviour
    {
        public RemoteObjectRepository repository { get; private set; }

        public void SetRepository(RemoteObjectRepository repository)
        {
            this.repository = repository;
        }

        void Awake()
        {
            DontDestroyOnLoad(this);    
        }
    }
}
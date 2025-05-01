using Domain.Stage.Object;
using UnityEngine;

namespace Infrastructure.Master
{
    public interface IMasterDataProvider
    {
        public InspectableObject GetInspectableObject(string id);
    }
}
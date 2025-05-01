using System.Collections.Generic;
using UnityEngine;
using Domain.Stage.Object;
using Infrastructure.Master;

namespace Infrastructure.Stage.Object
{
    public class InspectableObjectRepository
    {
        IMasterDataProvider provider;

        public InspectableObjectRepository(IMasterDataProvider provider) 
        {
            this.provider = provider;
        }

        public InspectableObject TryGetInspectableObject(string id)
        {
            var obj = provider.GetInspectableObject(id);

            return obj;
        }
    }
}


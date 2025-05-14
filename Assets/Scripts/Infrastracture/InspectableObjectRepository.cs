using System.Collections.Generic;
using UnityEngine;
using Domain.Stage.Object;
using Infrastructure.Master;
using Domain.Action;

namespace Infrastructure.Stage.Object
{
    public class InspectableObjectRepository
    {
        //一度取得したデータはキャッシュする
        //接続時にステージのデータを一括で受け取る

        IObjectDataProvider provider;
        List<List<ActionEntity>> AvailableActions = new();
        public InspectableObjectRepository() 
        {
        }
        public InspectableObject TryGetInspectableObject(string id)
        {
            var obj = provider.GetInspectableObject(id);

            return obj;
        }

        public InspectableObject TryGetCarryableObject(string id)
        {
            var obj = provider.GetCarryableObject(id);

            return obj;
        }


    }
}


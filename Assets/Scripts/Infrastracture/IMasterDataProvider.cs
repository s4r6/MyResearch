using System.Collections.Generic;
using Domain.Stage.Object;
using UnityEngine;
using Domain.Action;

namespace Infrastructure.Master
{
    public interface IObjectDataProvider
    {
        public InspectableObject GetInspectableObject(string id);
        public InspectableObject GetCarryableObject(string id);

    }

    public interface IActionDataProvider
    {
        public List<string> GetAvailableActionIds(string id);
        public List<ActionEntity> GetActionEntities(string id);

        public List<InspectableObject> GetActionableObjectsByObjectId(string id);
        public string? GetActionId(string label);
    }
}
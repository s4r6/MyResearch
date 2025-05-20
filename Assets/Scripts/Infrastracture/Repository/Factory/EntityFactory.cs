using Domain.Action;
using Domain.Component;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Domain.Stage.Object;
using Infrastructure.Repository;

namespace Infrastructure.Factory
{
    public interface IEntityFactory
    {
        ObjectEntity CreateEntityFromJson(JObject json);
    }
    public class EntityFactory : IEntityFactory
    {
        private readonly Dictionary<string, JObject> inspectableMap;
        private readonly Dictionary<string, JObject> actionHeldMap;
        private readonly HashSet<string> carryableSet;

        public EntityFactory()
        {
            inspectableMap = LoadAsMap("Master/InspectableComponents");
            actionHeldMap = LoadAsMap("Master/ActionHeldComponents");
            carryableSet = LoadList("Master/CarryableObjects");
        }

        public ObjectEntity CreateEntityFromJson(JObject json)
        {
            var id = json["ObjectId"]?.ToString();
            var components = json["Components"]?.ToObject<List<string>>() ?? new();

            var entity = new ObjectEntity(id);

            if (components.Contains("Inspectable") && inspectableMap.TryGetValue(id, out var inspectJson))
            {
                var inspectable = new InspectableComponent
                {
                    Description = inspectJson["Description"]?.ToString(),
                    Choices = ChoiceFactory.FromJsonArray((JArray)inspectJson["Choices"])
                };
                entity.Add(inspectable);
            }

            if(components.Contains("ActionHeld") && actionHeldMap.TryGetValue(id, out var actionHeldJson))
            {
                var needAttribute = actionHeldJson["NeedAttribute"]?.ToString();
                entity.Add(new ActionHeld(needAttribute));
            }

            if (components.Contains("Carryable") || carryableSet.Contains(id))
            {
                entity.Add(new CarryableComponent());
            }

            return entity;
        }

        // --- Utility JSON Loaders ---
        private Dictionary<string, JObject> LoadAsMap(string path)
        {
            Debug.Log(path);
            var json = Resources.Load<TextAsset>(path).text;
            var array = JsonConvert.DeserializeObject<JArray>(json);
            return array
                .OfType<JObject>()
                .ToDictionary(obj => obj["ObjectId"].ToString(), obj => obj);
        }

        private HashSet<string> LoadList(string path)
        {
            var json = Resources.Load<TextAsset>(path).text;
            return JsonConvert.DeserializeObject<List<string>>(json).ToHashSet();
        }
    }
}

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
            inspectableMap = LoadAsMap("Master/StageObjects");
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
                    DisplayName = inspectJson["DisplayName"]?.ToString(),
                    Description = inspectJson["Description"]?.ToString(),
                };

                var choices = ChoiceFactory.FromJsonArray((JArray)inspectJson["Choices"]);
                if(choices.Count > 0)
                {
                    var choicable = new ChoicableComponent
                    {
                        Choices = choices
                    };
                    entity.Add(choicable);
                }
                entity.Add(inspectable);
                
            }

            if(components.Contains("ActionHeld") && actionHeldMap.TryGetValue(id, out var actionHeldJson))
            {
                var needAttribute = actionHeldJson["NeedAttribute"]?.ToString();
                entity.Add(new ActionHeld(needAttribute));
            }

            if(components.Contains("Door"))
            {
                entity.Add(new DoorComponent());
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

using Domain.Stage.Object;
using Infrastructure.Factory;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UseCase.Player;

namespace Infrastructure.Repository
{
    public class ObjectRepository : IObjectRepository
    {
        private readonly Dictionary<string, ObjectEntity> entities = new();

        public ObjectRepository(IEntityFactory factory, string basePath = "Master/")
        {
            var rawObjects = LoadJson<JArray>(basePath + "Objects");
            foreach (var obj in rawObjects)
            {
                var entity = factory.CreateEntityFromJson((JObject)obj);
                Save(entity);
            }
        }

        public void Save(ObjectEntity entity)
        {
            entities[entity.Id] = entity;
        }

        public ObjectEntity GetById(string objectId)
        {
            return entities.TryGetValue(objectId, out var entity) ? entity : null;
        }

        public IReadOnlyList<ObjectEntity> GetAll() => entities.Values.ToList();

        private static JArray LoadJson<JArray>(string path)
        {
            var text = Resources.Load<TextAsset>(path).text;
            return Newtonsoft.Json.JsonConvert.DeserializeObject<JArray>(text);
        }

        public List<ObjectEntity> GetAllEntity()
        {
            return entities.Values.ToList();
        }
    }
}
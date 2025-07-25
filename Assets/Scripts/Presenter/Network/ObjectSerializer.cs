using System.Collections.Generic;
using System.Linq;
using Domain.Stage.Object;
using Infrastructure.Network;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Presenter.Network;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public static class ObjectSerializer
{
    public static List<ObjectEntity> ToEntities(JToken datas)
    {
        if(datas is JArray array)
        {
            return array.Select(ToEntity).Where(e => e != null).ToList()!;
        }

        if(datas.Type == JTokenType.Object)
        {
            var singleEntity = ToEntity(datas);
            return singleEntity != null ? new List<ObjectEntity> { singleEntity } : new();
        }

        Debug.LogWarning("Invalid token passed to ToEntities.");
        return new();
    }

    public static ObjectEntity? ToEntity(JToken? data)
    {
        if (data == null) return null;

        var objectId = data["objectId"]?.Value<string>();
        if (string.IsNullOrEmpty(objectId))
        {
            Debug.LogWarning("objectId missing in data object.");
            return null;
        }

        var entity = new ObjectEntity(objectId);

        foreach (var token in data["Components"] ?? Enumerable.Empty<JToken>())
        {
            var type = token["Type"]?.Value<string>();
            if (type == null) continue;

            IGameComponentDTO? dto = type switch
            {
                "ActionHeld" or "ActionSelf" => token.ToObject<ActionComponentData>(),
                "Carriable" => token.ToObject<CarriableComponentData>(),
                "Choicable" => token.ToObject<ChoicableComponentData>(),
                "Inspectable" => token.ToObject<InspectableComponentData>(),
                "Door" => token.ToObject<DoorComponentData>(),
                _ => null
            };

            if (dto == null)
            {
                Debug.LogWarning($"Unknown component type: {type}");
                continue;
            }

            var component = ComponentSerializer.ToComponent(dto);
            Debug.Log($"Added component type: {component.GetType().Name}");
            entity.Add(component);
        }

        return entity;
    }
}

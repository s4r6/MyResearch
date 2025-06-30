using System.Collections.Generic;
using Domain.Stage.Object;
using Infrastracture.Network;
using Infrastructure.Network;
using UniRx;
using Newtonsoft.Json.Linq;
using UseCase.Player;
using System.Linq;
using Presenter.Network;
using UnityEngine;


namespace Infrastructure.Repository
{
    public class RemoteObjectRepository : IObjectRepository
    {
        IWebSocketService server;
        Dictionary<string, ObjectEntity> entities = new();
        public RemoteObjectRepository(IWebSocketService server)
        {
            this.server = server;
            Bind();
        }

        public void Save(ObjectEntity entity)
        {
            entities.Add(entity.Id, entity);
        }

        public ObjectEntity GetById(string objectId)
        {
            return entities.TryGetValue(objectId, out var entity) ? entity : null;
        }

        public bool IsExist(string id)
        {
            return entities.ContainsKey(id);
        }

        public IReadOnlyList<ObjectEntity> GetAll() => entities.Values.ToList();

        void Bind()
        {
            server.OnMessage
                .Where(tuple => tuple.Item1 == PacketId.CreateRoomResponse)
                .Subscribe(tuple =>
                {
                    var payload = tuple.Item2["Payload"];

                    foreach (var sync in payload["SyncData"] ?? Enumerable.Empty<JToken>())
                    {
                        var entity = new ObjectEntity(sync["objectId"]!.Value<string>());
                        Save(entity);

                        foreach (var token in sync["Components"] ?? Enumerable.Empty<JToken>())
                        {
                            var type = token["Type"]?.Value<string>();

                            IGameComponentDTO dto = type switch
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
                                Debug.LogWarning($"Unknown component type: {token["Type"]}");
                                continue;
                            }

                            // DTO Å® DomainComponent Ç÷ïœä∑ÇµÅAEntity Ç…í«â¡
                            var component = ComponentSerializer.ToComponent(dto);
                            Debug.Log($"Added component type: {component.GetType().AssemblyQualifiedName}");
                            entity.Add(component);
                        }
                    }
                });

            server.OnMessage
                .Where(tuple => tuple.Item1 == PacketId.InspectObjectResponse)
                .Subscribe(tuple =>
                {
                    var payload = tuple.Item2["Payload"];
                    var data = payload["SyncData"] ?? null;

                    var entity = new ObjectEntity(data["objectId"]!.Value<string>());
                    if(IsExist(entity.Id))
                    {
                        entities.Remove(entity.Id);
                        Save(entity);
                    }

                    foreach (var token in data["Components"] ?? Enumerable.Empty<JToken>())
                    {
                        var type = token["Type"]?.Value<string>();

                        IGameComponentDTO dto = type switch
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
                            Debug.LogWarning($"Unknown component type: {token["Type"]}");
                            continue;
                        }

                        // DTO Å® DomainComponent Ç÷ïœä∑ÇµÅAEntity Ç…í«â¡
                        var component = ComponentSerializer.ToComponent(dto);
                        Debug.Log($"Added component type: {component.GetType().AssemblyQualifiedName}");
                        entity.Add(component);
                    }
                });
        }
    }
}
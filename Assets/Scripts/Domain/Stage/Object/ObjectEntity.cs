using System;
using System.Collections.Generic;
using Domain.Component;
using NUnit.Framework;
using UnityEngine;

namespace Domain.Stage.Object
{
    public class ObjectEntity
    {
        public string Id { get; }
        private Dictionary<Type, GameComponent> components = new();
        private readonly List<string> attributes = new();
        private readonly List<string> temporaryAttributes = new();

        public ObjectEntity(string id)
        {
            Id = id;
        }

        public void Add<T>(T component) where T : GameComponent
        {
            components[typeof(T)] = component;
        }

        public bool HasComponent<T>() where T : GameComponent
        {
            return components.ContainsKey(typeof(T));
        }

        public bool TryGetComponent<T>(out T component) where T : GameComponent
        {
            if (components.TryGetValue(typeof(T), out var comp))
            {
                component = (T)comp;
                return true;
            }
            component = null;
            return false;
        }

        public void AddPermanentAttribute(string attr) => attributes.Add(attr);
        public void AddTemporaryAttributes(IEnumerable<string> attrs) => temporaryAttributes.AddRange(attrs);
        public void ClearTemporaryAttributes() => temporaryAttributes.Clear();

        public bool HasAttribute(string attr) =>
            attributes.Contains(attr) || temporaryAttributes.Contains(attr);
    }
}
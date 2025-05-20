using Domain.Action;
using System.Collections.Generic;
using UnityEngine;

namespace Domain.Component
{
    public class ActionableComponent : GameComponent
    {
        public List<ActionAttribute> Attributes = new();

        public void AddAttribute(ActionAttribute attr)
        {
            Attributes.Add(attr);
        }
    }
}
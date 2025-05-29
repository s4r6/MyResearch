using Domain.Stage.Object;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Domain.Component
{
    public class InspectableComponent : GameComponent
    {
        public string DisplayName;
        public string Description;
        
        public bool IsActioned = false;
    }
}
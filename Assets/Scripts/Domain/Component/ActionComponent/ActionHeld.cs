using System.Collections.Generic;
using System.Linq;
using Domain.Action;
using Domain.Stage.Object;
using NUnit.Framework;
using UnityEngine;

namespace Domain.Component
{
    public class ActionHeld : GameComponent
    {
        readonly string NeedAttribute;

        public ActionHeld(string needAttribute)
        {
            NeedAttribute = needAttribute;
        }

        public bool IsMatch(ObjectEntity held)
        {
            if (!held.TryGetComponent<InspectableComponent>(out var inspectable)) return false;

            var HasAttribute = inspectable.SelectedChoice.OverrideActions
                                                            .Any(action => action.target == TargetType.HeldItem && action.ObjectAttributes.Contains(NeedAttribute));
            return HasAttribute;
        }

        public List<ActionEntity> GetAvailableActions(ObjectEntity held) 
        {
            if(!held.TryGetComponent<InspectableComponent>(out var inspectable)) return new();

            var availableActions = inspectable.SelectedChoice.OverrideActions
                                                                .FindAll(action => action.target == TargetType.HeldItem && action.ObjectAttributes.Contains(NeedAttribute));

            return availableActions;
        }
    }
}
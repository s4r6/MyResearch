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
            
            if (!held.TryGetComponent<ChoicableComponent>(out var choicable)) return false;

            var HasAttribute = choicable.SelectedChoice?.OverrideActions
                                                            .Any(action => action.target == TargetType.HeldItem && action.ObjectAttributes.Contains(NeedAttribute)) ?? false;
            return HasAttribute;
        }

        public List<ActionEntity> GetAvailableActions(ObjectEntity held) 
        {
            if(!held.TryGetComponent<ChoicableComponent>(out var choicable)) return new();

            var availableActions = choicable.SelectedChoice.OverrideActions
                                                                .FindAll(action => action.target == TargetType.HeldItem && action.ObjectAttributes.Contains(NeedAttribute));

            return availableActions;
        }
    }
}
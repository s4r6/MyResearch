using System.Collections.Generic;
using Domain.Action;
using Domain.Stage.Object;
using NUnit.Framework;
using UnityEngine;

namespace Domain.Component
{
    public class ActionSelf : GameComponent
    {
        public bool IsMatch(ObjectEntity self)
        {
            if (!self.TryGetComponent<InspectableComponent>(out var inspectable))
                return false;

            if(inspectable.SelectedChoice == null) return false;

            return true;
        }

        public List<ActionEntity> GetAvailableActions(ObjectEntity self)
        {
            if (!self.TryGetComponent<InspectableComponent>(out var inspectable))
                return new();

            if (inspectable.SelectedChoice == null) return new();

            var availableActions = inspectable.SelectedChoice.OverrideActions
                                                                .FindAll(action => action.target == TargetType.Self);

            return availableActions;
        }
    }
}
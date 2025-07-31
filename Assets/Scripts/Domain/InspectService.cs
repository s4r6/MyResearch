using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Action;
using Domain.Component;
using Domain.Stage.Object;

namespace Domain.Player
{
    public class InspectService
    {
        Dictionary<string, DateTime> FirstInspectTime = new(); 
        public bool CanInspect(ObjectEntity entity)
        {
            if(entity == null) return false;
            if (!entity.TryGetComponent<InspectableComponent>(out var inspectable)) return false;

            return true;
        }

        public ChoicableComponent TryGetChoice(ObjectEntity entity)
        {
            if(entity == null) return null;
            if (!entity.TryGetComponent<ChoicableComponent>(out var choicable)) return null;

            return choicable;
        }

        public void SetFirstInspectTime(string objectId)
        {
            if (!FirstInspectTime.ContainsKey(objectId)) 
            {
                FirstInspectTime.Add(objectId, DateTime.UtcNow);
            }
        }

        public TimeSpan CalcElapsedInspectTime(string objectId)
        {
            if(FirstInspectTime.TryGetValue(objectId, out var dateTime))
            {
                return DateTime.UtcNow - dateTime;
            }
            return TimeSpan.Zero;
        }

        public void ApplySelectedChoice(ObjectEntity entity, string selectedLabel) 
        {
            if (!entity.TryGetComponent<InspectableComponent>(out var inspectable)) return;
            if (!entity.TryGetComponent<ChoicableComponent>(out var choicable))  return;
            
            Choice choice = choicable.Choices.Find(x => x.Label == selectedLabel);

            //ActionComponent‚ðŽ‚Á‚Ä‚¢‚È‚¯‚ê‚Î
            if (!inspectable.IsActioned)
            {
                choicable.SelectedChoice = choice;
            }

            if (choicable.SelectedChoice.OverrideActions.Any(a => a.target == TargetType.Self) && !entity.HasComponent<ActionSelf>())
                entity.Add(new ActionSelf());

            return;
        }
    }
}
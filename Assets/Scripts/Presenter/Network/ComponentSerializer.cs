using Domain.Component;
using Infrastructure.Network;

namespace Presenter.Network
{
    public static class ComponentSerializer
    {
        public static GameComponent ToComponent(IGameComponentDTO component)
        {
            return component switch
            {
                ActionComponentData a when a.Type == "ActionHeld" => new ActionHeld(a.NeedAttribute),
                ActionComponentData a when a.Type == "ActionSelf" => new ActionSelf(),
                CarriableComponentData ca => new CarryableComponent(),
                ChoicableComponentData ch => new ChoicableComponent { Choices = ch.Choices, SelectedChoice = ch.SelectedChoice },
                DoorComponentData dr => new DoorComponent { IsOpen = dr.IsOpen, IsLock = dr.IsLock },
                InspectableComponentData ins => new Domain.Component.InspectableComponent { DisplayName = ins.DisplayName, Description = ins.Description, IsActioned = ins.IsActioned },
                _ => null
            };
        }
    }
}



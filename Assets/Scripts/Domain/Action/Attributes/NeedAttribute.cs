using System.Collections.Generic;
using System.Linq;
using Domain.Stage.Object;
using UnityEngine;

namespace Domain.Action
{
    public class NeedAttribute : ActionAttribute
    {

        private readonly List<string> requiredAttributes;
        private readonly TargetType attributeSource;

        public NeedAttribute(List<string> requiredAttributes, TargetType attributeSource, int priority)
        {
            this.requiredAttributes = requiredAttributes;
            this.attributeSource = attributeSource;
            this.Priority = priority;
        }

        public override bool IsMatch(ObjectEntity target, ObjectEntity heldItem)
        {
            //å¯â ëŒè€Ç≈ï™äÚ
            return attributeSource switch
            {
                //ëŒè€Ç™é©ï™ÇÃèÍçá => é©ï™ÇÃ
                TargetType.Self => requiredAttributes.All(attr => target.HasAttribute(attr)),
                TargetType.HeldItem => heldItem != null && requiredAttributes.All(attr => heldItem.HasAttribute(attr)),
                _ => false
            };
        }

        public override void ApplyEffect(ObjectEntity target, ObjectEntity heldItem)
        {
            // èåèÇÃÇ›Ç»ÇÃÇ≈âΩÇ‡ÇµÇ»Ç¢
        }
    }
}
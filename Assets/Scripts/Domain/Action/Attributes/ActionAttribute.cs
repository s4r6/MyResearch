using Domain.Stage.Object;
using UnityEngine;

namespace Domain.Action
{
    public abstract class ActionAttribute
    {
        public int Priority;

        public abstract bool IsMatch(ObjectEntity target, ObjectEntity heldItem);
        public abstract void ApplyEffect(ObjectEntity target, ObjectEntity heldItem);
    }
}
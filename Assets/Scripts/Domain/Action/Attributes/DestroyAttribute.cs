using Domain.Action;
using Domain.Stage.Object;
using UnityEngine;

public class DestroyAttribute : ActionAttribute
{
    public bool IsDestroy = false;
    public override bool IsMatch(ObjectEntity target, ObjectEntity heldItem)
    {
        throw new System.NotImplementedException();
    }

    public override void ApplyEffect(ObjectEntity target, ObjectEntity heldItem)
    {
        IsDestroy = true;
    }
}

using Domain.Tutorial;
using UnityEngine;

public class NoRestriction : IGameRestriction
{
    public bool CanEndGame() => true;

    public bool CanInspect() => true;

    public bool CanSelectRisk() => true;
}

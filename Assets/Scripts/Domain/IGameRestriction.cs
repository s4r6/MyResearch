using UnityEngine;

namespace Domain.Tutorial
{
    public interface IGameRestriction
    {
        bool CanInspect();
        bool CanSelectRisk();
        bool CanEndGame();
    }
}
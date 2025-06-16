using System;
using UnityEngine;

namespace UseCase.Player
{
    public interface IInspectUseCase
    {
        bool TryInspect(string objectId, Action onComplete);
        void OnEndInspect(string? choiceText);
    }
}
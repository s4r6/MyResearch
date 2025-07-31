using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UseCase.Player
{
    public interface IInspectUseCase
    {
        bool TryInspect(string objectId, Action onComplete);
        bool CanInspect(string objectId);
        UniTask OnEndInspect(string choiceText);
    }
}
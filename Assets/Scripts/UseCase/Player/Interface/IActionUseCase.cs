using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UseCase.Player
{
    public interface IActionUseCase
    {
        bool CanAction(string objectId);
        bool TryAction(string objectId, Action onComplete);
        UniTask OnEndSelectAction(string selectedActionLabel);
    }
}
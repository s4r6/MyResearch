using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IMoveController
{
    public UniTask OnMoveInput(Vector2 direction);
    public UniTask OnLookInput(Vector2 delta);
}

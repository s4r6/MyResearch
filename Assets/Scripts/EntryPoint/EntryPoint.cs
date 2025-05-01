using UnityEngine;
using UseCase.Player;
using View.Player;

namespace EntryPoint
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField]
        PlayerView view;
        [SerializeField]
        InputController input;

        PlayerMoveController useCase;
        void Awake()
        {
            useCase = new PlayerMoveController(view);
            input.usecase = useCase;
        }
    }
}


using System;
using Domain.Game;
using Domain.Player;
using UniRx;
using UnityEngine;
using View.Player;

namespace UseCase.Player
{
    public class PlayerSystemUseCase : IDisposable
    {
        InputController input;
        PlayerMoveController move;
        PlayerInspectUseCase inspect;
        PlayerEntity model;
        GameStateManager gameState;

        CompositeDisposable disposables = new CompositeDisposable();

        public PlayerSystemUseCase(PlayerMoveController move, PlayerInspectUseCase inspect, PlayerEntity model, InputController input, GameStateManager gameState) 
        { 
            this.move = move;
            this.inspect = inspect;
            this.input = input;
            this.model = model;
            this.gameState = gameState;

            Bind();
        }

        public void Update()
        {
            if(gameState.Current.IsMoving)
            {
                TryMove();
            }
            
            inspect.Update();
        }

        public void LateUpdate()
        {
            if(gameState.Current.IsMoving)
            {
                TryLook();
            }
        }

        void Bind()
        {
            input.OnInspectButtonPressed
                .Subscribe(_ =>
                {
                    Debug.Log("OnInspect");
                    var result = inspect.TryInspect(() =>
                    {
                        gameState.Set(GamePhase.Moving);
                    });

                    if(result)
                    {
                        gameState.Set(GamePhase.Inspecting);  
                    }
                }).AddTo(disposables);
        }

        void TryMove()
        {
            var direction = input.GetMoveInput();
            move.OnMoveInput(direction);
        }

        void TryLook()
        {
            var delta = input.GetLookInput();
            move.OnLookInput(delta);
        }

        public void Dispose() 
        { 
            disposables.Dispose();
        }

    }
}


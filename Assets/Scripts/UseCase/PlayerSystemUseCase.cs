using System;
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

        CompositeDisposable disposables = new CompositeDisposable();

        public PlayerSystemUseCase(PlayerMoveController move, PlayerInspectUseCase inspect, PlayerEntity model, InputController input) 
        { 
            this.move = move;
            this.inspect = inspect;
            this.input = input;
            this.model = model;

            model.isMovable = true;
            model.isLookable = true;

            Bind();
        }

        public void Update()
        {
            var direction = input.GetMoveInput();
            move.OnMoveInput(direction);
            inspect.Update();
        }

        public void LateUpdate()
        {
            var delta = input.GetLookInput();
            move.OnLookInput(delta);
        }

        void Bind()
        {
            input.OnInspectButtonPressed
                .Subscribe(x =>
                {
                    Debug.Log("OnInspect");
                    inspect.TryInspect();
                }).AddTo(disposables);
        }

        public void Dispose() 
        { 
            disposables.Dispose();
        }

    }
}


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
        PlayerCarryUseCase carry;
        PlayerActionUseCase action;
        PlayerEntity model;
        GameStateManager gameState;
        RaycastController raycast;

        CompositeDisposable disposables = new CompositeDisposable();

        public PlayerSystemUseCase(
            PlayerMoveController move,
            PlayerInspectUseCase inspect,
            PlayerEntity model,
            InputController input,
            GameStateManager gameState,
            RaycastController raycast,
            PlayerCarryUseCase carry,
            PlayerActionUseCase action)
        {
            this.move = move;
            this.inspect = inspect;
            this.input = input;
            this.model = model;
            this.gameState = gameState;
            this.raycast = raycast;
            this.carry = carry;
            this.action = action;

            Bind();
        }

        private void Bind()
        {
            input.OnInspectButtonPressed
                .Subscribe(_ =>
                {
                    Debug.Log("[PlayerSystemUseCase] 検査ボタンが押されました");
                    var objectId = model.currentLookingObject;
                    var result = inspect.TryInspect(objectId, () =>
                    {
                        gameState.Set(GamePhase.Moving);
                    });

                    if(result)
                    {
                        gameState.Set(GamePhase.Inspecting);
                    }
                }).AddTo(disposables);

            input.OnPickUpButtonPressed
                .Subscribe(_ =>
                {
                    Debug.Log("[PlayerSystemUseCase] 拾う/置くボタンが押されました");
                    if (string.IsNullOrEmpty(model.currentCarringObject))
                    {
                        string objectId = model.currentLookingObject;
                        carry.TryPickUp(objectId);
                    }
                    else
                    {
                        carry.TryDrop();
                    }
                }).AddTo(disposables);

            input.OnActionButtonPressed
                .Subscribe(_ =>
                {
                    Debug.Log("[PlayerSystemUseCase] アクションボタンが押されました");
                    var objectId = model.currentLookingObject;
                    var result = action.TryAction(objectId, () =>
                    {
                        gameState.Set(GamePhase.Moving);
                    });

                    if(result)
                    {
                        gameState.Set(GamePhase.SelectAction);
                    }

                }).AddTo(disposables);
        }

        public void Update()
        {
            if(gameState.Current.IsMoving)
            {
                TryMove();
                GetLookingObjectId();
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

        private void GetLookingObjectId()
        {
            var name = raycast.TryGetLookedObjectId();
            model.currentLookingObject = name;
        }

        private void TryMove()
        {
            var direction = input.GetMoveInput();
            move.OnMoveInput(direction);
        }

        private void TryLook()
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


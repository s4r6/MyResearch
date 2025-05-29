using System;
using Domain.Action;
using Domain.Component;
using Domain.Game;
using Domain.Player;
using Domain.Stage;
using UniRx;
using UnityEngine;
using UseCase.Game;
using View.Player;

namespace UseCase.Player
{
    public struct ActionHistory
    {
        public string ObjectName { get; }
        public string SelectedRiskLable { get; }
        public string ExecutedActionLabel { get; }

        public int RiskChange { get; } // 実行による変化量

        public int ActionCost { get; } // 使用したアクションポイント

        public ActionHistory(
            string objectName,
            string riskLabel,
            string actionLabel,
            int riskChange,
            int actionCost)
        {
            ObjectName = objectName;
            SelectedRiskLable = riskLabel;
            ExecutedActionLabel = actionLabel;
            RiskChange = riskChange;
            ActionCost = actionCost;
        }
    }
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
        InteractUseCase interact;
        

        CompositeDisposable disposables = new CompositeDisposable();

        public Subject<ActionHistory> OnActionExecute = new();
        public Subject<Unit> OnExitPointInspected = new();

        public PlayerSystemUseCase(
            PlayerMoveController move,
            PlayerInspectUseCase inspect,
            PlayerEntity model,
            InputController input,
            GameStateManager gameState,
            RaycastController raycast,
            PlayerCarryUseCase carry,
            PlayerActionUseCase action,
            InteractUseCase interact
            )
        {
            this.move = move;
            this.inspect = inspect;
            this.input = input;
            this.model = model;
            this.gameState = gameState;
            this.raycast = raycast;
            this.carry = carry;
            this.action = action;
            this.interact = interact;


            Bind();
        }

        private void Bind()
        {
            input.OnInspectButtonPressed
                .Subscribe(_ =>
                {

                    var objectId = model.currentLookingObject;
                    if (objectId == "DoorOutside")
                    {
                        OnExitPointInspected.OnNext(default);
                        return;
                    }

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
                    var objectId = model.currentLookingObject;
                    var result = action.TryAction(objectId, result =>
                    {
                        gameState.Set(GamePhase.Moving);

                        var actionEntity = result.Item1;
                        var targetObject = result.Item2;

                        if (actionEntity == null || targetObject == null)
                            return;

                        if (!targetObject.TryGetComponent<ChoicableComponent>(out var choicable))
                            return;

                        var selectedRiskLabel = choicable.SelectedChoice.Label;
                        var history = new ActionHistory(targetObject.Id, selectedRiskLabel, actionEntity.label, actionEntity.riskChange, actionEntity.actionPointCost);

                        OnActionExecute.OnNext(history);
                        
                    });

                    if(result)
                    {
                        gameState.Set(GamePhase.SelectAction);
                    }

                }).AddTo(disposables);

            input.OnInteractButtonPressed
                .Subscribe(_ => 
                {
                    var objectId = model.currentLookingObject;
                    interact.TryInteract(objectId);
                }).AddTo(disposables);

            input.OnFinishButtonPressed
                .Subscribe(_ =>
                {
                    OnExitPointInspected.OnNext(default);
                }).AddTo(disposables);

            
        }

        public void Update()
        {
            if(gameState.Current.IsMoving)
            {
                TryMove();
                GetLookingObjectId();
            }
        }

        public void LateUpdate()
        {
            if(gameState.Current.IsMoving)
            {
                TryLook();
            }
        }

        public void StartGame()
        {
            Debug.Log("ゲームスタート");
        }

        public void EndGame()
        {
            Dispose();
        }

        public void DoAction()
        {

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


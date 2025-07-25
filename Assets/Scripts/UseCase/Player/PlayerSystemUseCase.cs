using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Domain.Action;
using Domain.Component;
using Domain.Game;
using Domain.Player;
using Domain.Stage;
using NUnit.Framework;
using UniRx;
using UnityEngine;
using UseCase.Game;
using UseCase.Network.DTO;
using View.Player;

namespace UseCase.Player
{
    //操作説明を表示する
    public interface IActionHintPresenter
    {
        void ShowAvailableActions(IEnumerable<ActionHint> hints);
    }
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
        IMoveController move;
        IInspectUseCase inspect;
        PlayerCarryUseCase carry;
        IActionUseCase action;
        PlayerEntity model;
        GameStateManager gameState;
        RaycastController raycast;
        InteractUseCase interact;
        IActionHintPresenter hintPresenter;
        

        CompositeDisposable disposables = new CompositeDisposable();

        public Subject<Unit> OnExitPointInspected = new();

        public PlayerSystemUseCase(
            IMoveController move,
            IInspectUseCase inspect,
            PlayerEntity model,
            InputController input,
            GameStateManager gameState,
            RaycastController raycast,
            PlayerCarryUseCase carry,
            IActionUseCase action,
            InteractUseCase interact,
            IActionHintPresenter hintPresenter
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
            this.hintPresenter = hintPresenter;


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
                    var result = action.TryAction(objectId, () =>
                    {
                        gameState.Set(GamePhase.Moving);   
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

        public async UniTask Update()
        {
            UpdateAvailableActions();
            if(gameState.Current.IsMoving)
            {
                await TryMove();
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

        private void GetLookingObjectId()
        {
            var name = raycast.TryGetLookedObjectId();
            model.currentLookingObject = name;
        }

        private async UniTask TryMove()
        {
            var direction = input.GetMoveInput();
            await move.OnMoveInput(direction);
        }

        private void TryLook()
        {
            var delta = input.GetLookInput();
            move.OnLookInput(delta);
        }

        void UpdateAvailableActions()
        {
            var hints = new List<ActionHint>();
            string objectId = model.currentLookingObject;
            GamePhase phase = gameState.Current.Phase;

            //MovingPhase
            if (gameState.Current.IsMoving)
            {
                hints.Add(new ActionHint(ActionHintId.Move));
                hints.Add(new ActionHint(ActionHintId.Document));
                if (action.CanAction(objectId))
                    hints.Add(new ActionHint(ActionHintId.Action));
                if (inspect.CanInspect(objectId))
                    hints.Add(new ActionHint(ActionHintId.Inspect));
                if (interact.CanInteract(objectId))
                    hints.Add(new ActionHint(ActionHintId.Interact));
            }
            else if (gameState.Current.IsInspecting)
            {
                hints.Add(new ActionHint(ActionHintId.SelectRisk));
                hints.Add(new ActionHint(ActionHintId.Select));
                hints.Add(new ActionHint(ActionHintId.Cancel));
            }
            else if (gameState.Current.IsSelectingAction) 
            {
                hints.Add(new ActionHint(ActionHintId.SelectAction));
                hints.Add(new ActionHint(ActionHintId.Select));
                hints.Add(new ActionHint(ActionHintId.Cancel));
            }

                hintPresenter.ShowAvailableActions(hints);
            
        }
        public void Dispose()
        {
            disposables.Dispose();
        }
    }
}


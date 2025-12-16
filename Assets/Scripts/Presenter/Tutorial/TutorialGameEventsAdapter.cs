using System;
using UnityEngine;
using UseCase.Game;
using UseCase.Player;
using View.UI;
using UniRx;
using Domain.Player;
using UseCase.GameSystem;

public class TutorialGameEventsAdapter : ITutorialGameEvents, IDisposable
{
    public event Action EnvironmentDocumentOpened;
    public event Action EnvironmentDocumentClosed;
    public event Action<string> ObjectInspected;
    public event Action<string> RiskSelected;
    public event Action<string> ActionListOpened;
    public event Action<string> ActionSelected;
    public event Action<string> ObjectHeld;
    public event Action EndGame;

    readonly DocumentView _document;
    readonly PlayerInspectUseCase _inspectUseCase;
    readonly PlayerActionUseCase _actionUseCase;
    readonly PlayerCarryUseCase _carryUseCase;
    readonly GameSystemUseCase _game;

    CompositeDisposable _disposables = new CompositeDisposable();
    public TutorialGameEventsAdapter(
        DocumentView document,
        PlayerInspectUseCase inspectUseCase,
        PlayerActionUseCase actionUseCase,
        PlayerCarryUseCase carryUseCase,
        GameSystemUseCase game)
    {
        _document = document;
        _inspectUseCase = inspectUseCase;
        _actionUseCase = actionUseCase;
        _carryUseCase = carryUseCase;
        _game = game;

        _document.DocumentOpened += OnDocumentOpened;
        _document.DocumentClosed += OnDocumentClosed;

        _inspectUseCase.OnInspected
            .Subscribe(x =>
            {
                OnObjectInspectedInternal(x);
            }).AddTo(_disposables);

        _inspectUseCase.OnRiskSelected
            .Subscribe(x =>
            {
                OnRiskSelectedInternal(x);
            }).AddTo(_disposables);

        _actionUseCase.OnActionListOpened
            .Subscribe(_ =>
            {
                OnActionListOpenedInternal();
            }).AddTo(_disposables);

        _actionUseCase.OnActionExecuted
            .Subscribe(_ =>
            {
                OnActionExecutedInternal();
            }).AddTo(_disposables);

        _carryUseCase.OnObjectHeld
            .Subscribe(x =>
            {
                OnHeldObject(x);
            }).AddTo(_disposables);

        _game.OnEndGame
            .Subscribe(x =>
            {
                OnEndGame();  
            }).AddTo(_disposables);
    }

    void OnDocumentOpened()
    {
        EnvironmentDocumentOpened?.Invoke();   
    }

    void OnDocumentClosed()
    {
        EnvironmentDocumentClosed?.Invoke();
    }

    void OnObjectInspectedInternal(string objectId)
    {
        // そのまま流す（Phase2 では Flow側で対象IDをチェック）
        ObjectInspected?.Invoke(objectId);
    }

    void OnRiskSelectedInternal(string objectId)
    {
        RiskSelected?.Invoke(objectId);
    }

    void OnActionListOpenedInternal()
    {
        ActionListOpened?.Invoke("");
    }

    void OnActionExecutedInternal()
    {
        ActionSelected?.Invoke("");
    }

    void OnHeldObject(string id)
    {
        ObjectHeld?.Invoke(id);
    }

    void OnEndGame()
    {
        EndGame?.Invoke();
    }

    public void Dispose()
    {
        _document.DocumentOpened -= OnDocumentOpened;
        _document.DocumentClosed -= OnDocumentClosed;

        _disposables.Dispose();
    }
}

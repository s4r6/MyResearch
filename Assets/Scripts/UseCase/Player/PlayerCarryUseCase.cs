using UnityEngine;
using Domain.Stage.Object;
using Presenter.Player;
using UseCase.Player;
using Domain.Component;
using UniRx;
using System;

namespace Domain.Player
{
    public class PlayerCarryUseCase
    {
        PlayerEntity model;
        IObjectRepository repository;
        PlayerCarryPresenter presenter;

        Subject<string> onObjectHeld = new();
        public IObservable<string> OnObjectHeld => onObjectHeld;
        public PlayerCarryUseCase(PlayerEntity model, PlayerCarryPresenter presenter, IObjectRepository repository)
        {
            this.model = model;
            this.repository = repository;
            this.presenter = presenter;
        }

        public bool IsPickable(string objectId)
        {
            var entity = repository.GetById(objectId);
            if (entity == null) return false;
            return entity.HasComponent<CarryableComponent>();
        }

        public bool TryPickUp(string objectId)
        {
            if (!IsPickable(objectId))
                return false;

            // PlayerEntityにアイテムを保存
            model.currentCarringObject = objectId;

            // Presenterを通じてViewに表示を依頼
            presenter.HoldObject(objectId);

            onObjectHeld.OnNext(objectId);
            return true; // 拾うことに成功した場合はtrueを返す
            
        }
        
        public bool TryDrop()
        {
            if (string.IsNullOrEmpty(model.currentCarringObject))
            {
                return false;
            }
            
            // アイテムを置く処理
            string objectId = model.currentCarringObject;
            
            // PlayerEntityからアイテムを削除
            model.currentCarringObject = string.Empty;
            
            // Presenterを通じてViewに表示を依頼
            presenter.ReleaseObject(objectId);
            
            return true; // 置くことに成功した場合はtrueを返す
        }
    }
}
using UnityEngine;
using Domain.Stage.Object;
using Presenter.Player;
using UseCase.Player;
using Domain.Component;

namespace Domain.Player
{
    public class PlayerCarryUseCase
    {
        PlayerEntity model;
        IObjectRepository repository;
        PlayerCarryPresenter presenter;

        public PlayerCarryUseCase(PlayerEntity model, PlayerCarryPresenter presenter, IObjectRepository repository)
        {
            this.model = model;
            this.repository = repository;
            this.presenter = presenter;
            Debug.Log("[CarryUseCase] 初期化完了");
        }

        public bool TryPickUp(string objectId)
        {
            var entity = repository.GetById(objectId);
            if(entity.HasComponent<CarryableComponent>())
            {
                // PlayerEntityにアイテムを保存
                model.currentCarringObject = objectId;
                Debug.Log($"[CarryUseCase] モデルにオブジェクト({objectId})を設定");

                // Presenterを通じてViewに表示を依頼
                Debug.Log($"[CarryUseCase] Presenterに処理を委譲");
                presenter.HoldObject(objectId);

                Debug.Log($"[CarryUseCase] TryPickUp 完了: objectId={objectId}");
                return true; // 拾うことに成功した場合はtrueを返す
            }

            return false;
            
        }
        
        public bool TryDrop()
        {
            Debug.Log("[CarryUseCase] TryDrop 開始");
            if (string.IsNullOrEmpty(model.currentCarringObject))
            {
                Debug.Log("[CarryUseCase] 持っているオブジェクトがないため処理を中断");
                return false;
            }
            
            // アイテムを置く処理
            string objectId = model.currentCarringObject;
            Debug.Log($"[CarryUseCase] アイテムを置きます: {objectId}");
            
            // PlayerEntityからアイテムを削除
            model.currentCarringObject = string.Empty;
            Debug.Log($"[CarryUseCase] モデルからオブジェクト({objectId})を削除");
            
            // Presenterを通じてViewに表示を依頼
            Debug.Log($"[CarryUseCase] Presenterに処理を委譲");
            presenter.ReleaseObject(objectId);
            
            Debug.Log($"[CarryUseCase] TryDrop 完了: objectId={objectId}");
            return true; // 置くことに成功した場合はtrueを返す
        }
    }
}
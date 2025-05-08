using UnityEngine;
using View.Player;

namespace Presenter.Player
{
    public class PlayerCarryPresenter
    {
        private readonly IPlayerCarryView view;

        public PlayerCarryPresenter(IPlayerCarryView view)
        {
            this.view = view;
            Debug.Log("[CarryPresenter] 初期化完了");
        }

        public void HoldObject(string objectId)
        {
            Debug.Log($"[CarryPresenter] HoldObject 開始: objectId={objectId}");
            if (string.IsNullOrEmpty(objectId))
            {
                Debug.Log("[CarryPresenter] objectIdが無効なため処理を中断");
                return;
            }
            
            Debug.Log($"[CarryPresenter] Viewに処理を委譲: objectId={objectId}");
            view.HoldObject(objectId);
            Debug.Log($"[CarryPresenter] オブジェクト {objectId} を持ちました");
            Debug.Log($"[CarryPresenter] HoldObject 完了: objectId={objectId}");
        }

        public void ReleaseObject(string objectId)
        {
            Debug.Log($"[CarryPresenter] ReleaseObject 開始: objectId={objectId}");
            if (string.IsNullOrEmpty(objectId))
            {
                Debug.Log("[CarryPresenter] objectIdが無効なため処理を中断");
                return;
            }
            
            Debug.Log($"[CarryPresenter] Viewに処理を委譲: objectId={objectId}");
            view.ReleaseObject(objectId);
            Debug.Log($"[CarryPresenter] オブジェクト {objectId} を離しました");
            Debug.Log($"[CarryPresenter] ReleaseObject 完了: objectId={objectId}");
        }
    }
} 
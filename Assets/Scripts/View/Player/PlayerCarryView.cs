using System.Collections.Generic;
using UnityEngine;

namespace View.Player
{
    public class PlayerCarryView : MonoBehaviour, IPlayerCarryView
    {
        [SerializeField] private Transform handTransform; // プレイヤーの手の位置
        [SerializeField] private LayerMask carryableObjectLayer; // 持ち運び可能なオブジェクトのレイヤー
        
        private Dictionary<string, GameObject> carryableObjects = new Dictionary<string, GameObject>();
        private GameObject currentHeldObject = null;

        private void Start()
        {
            Debug.Log("[CarryView] Start 開始");
            // シーン内の持ち運び可能なオブジェクトを検索して辞書に追加
            GameObject[] sceneObjects = GameObject.FindObjectsOfType<GameObject>();
            
            int addedCount = 0;
            foreach (GameObject obj in sceneObjects)
            {
                // 指定したレイヤーに属するオブジェクトのみを追加
                if (((1 << obj.layer) & carryableObjectLayer) != 0)
                {
                    carryableObjects[obj.name] = obj;
                    addedCount++;
                }
            }
        }

        public void HoldObject(string objectId)
        {
            Debug.Log($"[CarryView] HoldObject 開始: objectId={objectId}");
            
            if (currentHeldObject != null)
            {
                // すでに何かを持っている場合は先に離す
                Debug.Log($"[CarryView] 既に持っているオブジェクト({currentHeldObject.name})があるため、先に離します");
                ReleaseObject(currentHeldObject.name);
            }

            if (carryableObjects.TryGetValue(objectId, out GameObject targetObject))
            {
                Debug.Log($"[CarryView] 対象オブジェクト({objectId})を見つけました");
                
                // 物理挙動を無効化
                Rigidbody rb = targetObject.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = true;
                    Debug.Log($"[CarryView] Rigidbodyを無効化: isKinematic={rb.isKinematic}");
                }
                else
                {
                    Debug.Log($"[CarryView] 対象オブジェクトにRigidbodyがありません");
                }
                
                // コライダーを無効化
                Collider[] colliders = targetObject.GetComponents<Collider>();
                Debug.Log($"[CarryView] コライダー数: {colliders.Length}");
                foreach (Collider collider in colliders)
                {
                    collider.enabled = false;
                    Debug.Log($"[CarryView] コライダーを無効化: {collider.GetType().Name}");
                }
                
                // オブジェクトを手の位置に移動
                Vector3 originalPosition = targetObject.transform.position;
                Quaternion originalRotation = targetObject.transform.rotation;
                
                targetObject.transform.SetParent(handTransform);
                targetObject.transform.localPosition = Vector3.zero;
                targetObject.transform.localRotation = Quaternion.identity;
                
                Debug.Log($"[CarryView] オブジェクトを移動: 元位置={originalPosition}, 移動後={targetObject.transform.position}");
                
                currentHeldObject = targetObject;
                Debug.Log($"[CarryView] オブジェクト {objectId} を持ちました");
            }
            else
            {
                Debug.LogWarning($"[CarryView] 持ち運び可能なオブジェクト {objectId} が見つかりません。登録済みオブジェクト数: {carryableObjects.Count}");
            }
            
            Debug.Log($"[CarryView] HoldObject 完了: objectId={objectId}");
        }

        public void ReleaseObject(string objectId)
        {
            Debug.Log($"[CarryView] ReleaseObject 開始: objectId={objectId}");
            
            if (currentHeldObject == null)
            {
                Debug.Log("[CarryView] 現在持っているオブジェクトがないため処理を中断");
                return;
            }
            
            if (currentHeldObject.name != objectId)
            {
                Debug.Log($"[CarryView] 指定されたオブジェクト({objectId})と現在持っているオブジェクト({currentHeldObject.name})が一致しないため処理を中断");
                return;
            }

            // 親子関係を解除
            Vector3 positionBeforeRelease = currentHeldObject.transform.position;
            currentHeldObject.transform.SetParent(null);
            Debug.Log($"[CarryView] 親子関係を解除: 位置変化={positionBeforeRelease} -> {currentHeldObject.transform.position}");
            
            // 物理挙動を再開
            Rigidbody rb = currentHeldObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                Debug.Log($"[CarryView] Rigidbodyを有効化: isKinematic={rb.isKinematic}");
            }
            else
            {
                Debug.Log($"[CarryView] 対象オブジェクトにRigidbodyがありません");
            }
            
            // コライダーを再開
            Collider[] colliders = currentHeldObject.GetComponents<Collider>();
            Debug.Log($"[CarryView] コライダー数: {colliders.Length}");
            foreach (Collider collider in colliders)
            {
                collider.enabled = true;
                Debug.Log($"[CarryView] コライダーを有効化: {collider.GetType().Name}");
            }
            
            Debug.Log($"[CarryView] オブジェクト {objectId} を離しました");
            currentHeldObject = null;
            
            Debug.Log($"[CarryView] ReleaseObject 完了: objectId={objectId}");
        }
    }
} 
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
            
            if (currentHeldObject != null)
            {
                // すでに何かを持っている場合は先に離す

                ReleaseObject(currentHeldObject.name);
            }

            if (carryableObjects.TryGetValue(objectId, out GameObject targetObject))
            {

                
                // 物理挙動を無効化
                Rigidbody rb = targetObject.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = true;

                }

                
                // コライダーを無効化
                Collider[] colliders = targetObject.GetComponents<Collider>();

                foreach (Collider collider in colliders)
                {
                    collider.enabled = false;
                }
                
                // オブジェクトを手の位置に移動
                Vector3 originalPosition = targetObject.transform.position;
                Quaternion originalRotation = targetObject.transform.rotation;
                
                targetObject.transform.SetParent(handTransform);
                targetObject.transform.localPosition = Vector3.zero;
                targetObject.transform.localRotation = Quaternion.identity;
                

                
                currentHeldObject = targetObject;

            }


        }

        public void ReleaseObject(string objectId)
        {

            
            if (currentHeldObject == null)
            {
                return;
            }
            
            if (currentHeldObject.name != objectId)
            {
                
                return;
            }

            // 親子関係を解除
            Vector3 positionBeforeRelease = currentHeldObject.transform.position;
            currentHeldObject.transform.SetParent(null);

            
            // 物理挙動を再開
            Rigidbody rb = currentHeldObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;

            }

            
            // コライダーを再開
            Collider[] colliders = currentHeldObject.GetComponents<Collider>();

            foreach (Collider collider in colliders)
            {
                collider.enabled = true;

            }

            currentHeldObject = null;

        }
    }
} 
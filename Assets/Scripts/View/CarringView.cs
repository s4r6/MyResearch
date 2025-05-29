using UnityEngine;

namespace View.Player
{
    public class CarringView : MonoBehaviour
    {
        [SerializeField] private Transform handTransform;
        [SerializeField] private float followSpeed = 5f;

        private GameObject target;
        private Vector3 velocity;


        //---------------------VIEW--------------------------//
        public void SetTarget(GameObject obj)
        {
            target = obj;
            obj.GetComponent<Rigidbody>().isKinematic = true;
        }

        public void ClearTarget()
        {
            if (target != null)
            {
                target.GetComponent<Rigidbody>().isKinematic = false;
                target = null;
            }
        }

        private void LateUpdate()
        {
            if (target != null)
            {
                Vector3 desiredPosition = handTransform.position;
                target.transform.position = Vector3.SmoothDamp(
                    target.transform.position,
                    desiredPosition,
                    ref velocity,
                    1f / followSpeed
                );

                // ÉIÉvÉVÉáÉì: âÒì]Ç‡í«è]
                target.transform.rotation = Quaternion.Slerp(
                    target.transform.rotation,
                    handTransform.rotation,
                    Time.deltaTime * followSpeed
                );
            }
        }

        //---------------------PRESENTER--------------------------//
        public void AttachObject(GameObject obj)
        {
            SetTarget(obj);
        }

        public void DetachObject()
        {
            ClearTarget();
        }
    }
}
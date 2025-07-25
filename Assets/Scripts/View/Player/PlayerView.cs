using System;
using UnityEngine;
using UnityEngine.EventSystems;


namespace View.Player
{
    public class PlayerView : MonoBehaviour
    {
        [SerializeField]
        CharacterController controller;
        [SerializeField]
        Transform cameraTramsform;

        public Vector3 Position => transform.position;
        public Quaternion Rotation => transform.rotation;

        float pitchAngle = 0f;
        float SpeedChangeRate = 10.0f;

        [Header("Mouse Cursor Settings")]
        public static bool cursorLocked = true;
        public static bool cursorInputForLook = true;
        void OnApplicationFocus(bool hasFocus)
        {
            SetCursorState(cursorLocked);
        }

        void SetCursorState(bool newState)
        {
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
        }

        public Vector3 Move(Vector2 inputDirection, float moveSpeed)
        {
            if (inputDirection == Vector2.zero)
                return controller.transform.position;

            float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0f, controller.velocity.z).magnitude;
            float targetSpeed = moveSpeed;
            float speedOffset = 0.1f;
            float speed = (Mathf.Abs(currentHorizontalSpeed - targetSpeed) > speedOffset)
                ? Mathf.Lerp(currentHorizontalSpeed, targetSpeed, Time.deltaTime * SpeedChangeRate)
                : targetSpeed;

            speed = Mathf.Round(speed * 1000f) / 1000f;

            Vector3 moveDir = (transform.right * inputDirection.x + transform.forward * inputDirection.y).normalized;
            controller.Move(moveDir * speed * Time.deltaTime);

            return controller.transform.position;
        }

        public Quaternion Rotate(float yaw, float pitch) 
        {
            transform.Rotate(Vector3.up * yaw);

            pitchAngle -= pitch;
            pitchAngle = Mathf.Clamp(pitchAngle, -89f, 89f);

            cameraTramsform.localRotation = Quaternion.Euler(pitchAngle, 0f, 0f);

            return transform.rotation;
        }
    }

}

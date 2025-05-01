using UnityEngine;

namespace Domain.Player
{
    public class PlayerEntity
    {
        public readonly string id;
        public Vector3 position;
        public Quaternion rotation;
        
        public float speed = 20;

        public Vector2 lookSensitivity = new Vector2(0.2f, 0.2f);

        public PlayerEntity(Vector3 position, Quaternion rotation)
        {
            this.position = position;
            this.rotation = rotation;
        }

        public void SetPosition(Vector3 position)
        {
            this.position = position;
        }

        public void SetRotation(Quaternion rotation)
        {
            this.rotation = rotation;
        }
    }
}


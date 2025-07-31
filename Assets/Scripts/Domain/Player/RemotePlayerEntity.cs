using UnityEngine;

namespace Domain.Network
{
    public readonly struct Position
    {
        public float X { get; }
        public float Y { get; }
        public float Z { get; }

        public Position(float x, float y, float z)
        {
            X = x; Y = y; Z = z;
        }

        public static Position Lerp(Position a, Position b, float t)
        {
            return new Position(
                Mathf.Lerp(a.X, b.X, t),
                Mathf.Lerp(a.Y, b.Y, t),
                Mathf.Lerp(a.Z, b.Z, t)
            );
        }

        public Vector3 ToVector3() => new Vector3(X, Y, Z);
        public static Position FromVector3(Vector3 vec) => new Position(vec.x, vec.y, vec.z);
    }

    public class RemotePlayerEntity
    {
        public string Id {  get; }
        Position current;

        public RemotePlayerEntity(string id, Position pos)
        {
            Id = id;
            current = pos;
        }

        public void SetPosition(Position newPos)
        {
            current = newPos;
        }

        public Position GetPosition() => current;
    }
}
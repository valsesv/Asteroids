using UnityEngine;

namespace Asteroids.Core.Entity.Components
{
    public class TransformComponent : IComponent
    {
        public Vector2 Position { get; private set; }
        public float Rotation { get; private set; }

        public TransformComponent(Vector2 position = default, float rotation = 0f)
        {
            Position = position;
            Rotation = rotation;
        }

        public void SetPosition(Vector2 position)
        {
            Position = position;
        }

        public void SetRotation(float rotation)
        {
            Rotation = rotation;
        }

        public void Move(Vector2 delta)
        {
            Position += delta;
        }
    }
}
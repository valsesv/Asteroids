using UnityEngine;

namespace Asteroids.Core.Entity.Components
{
    /// <summary>
    /// Component for spatial transformation data (position, rotation)
    /// </summary>
    public class TransformComponent : IComponent
    {
        public Vector2 Position { get; private set; }
        public float Rotation { get; private set; }

        public TransformComponent(Vector2 position = default, float rotation = 0f)
        {
            Position = position;
            Rotation = rotation;
        }
    }
}


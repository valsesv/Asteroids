using Asteroids.Core.Entity;

namespace Asteroids.Core.Player
{
    /// <summary>
    /// Component that identifies an entity as a bullet
    /// </summary>
    public class BulletComponent : IComponent
    {
        public float Lifetime { get; set; }
        public float Speed { get; set; }

        public BulletComponent(float speed, float lifetime)
        {
            Speed = speed;
            Lifetime = lifetime;
        }
    }
}


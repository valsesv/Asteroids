namespace Asteroids.Core.Entity.Components
{
    /// <summary>
    /// Signal fired when transform component changes (position or rotation)
    /// Generic signal that can be used by any entity with TransformComponent
    /// </summary>
    public class TransformChangedSignal
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Rotation { get; set; }
    }

    /// <summary>
    /// Signal fired when physics component changes (velocity)
    /// Generic signal that can be used by any entity with PhysicsComponent
    /// </summary>
    public class PhysicsChangedSignal
    {
        public float VelocityX { get; set; }
        public float VelocityY { get; set; }
        public float Speed { get; set; }
    }

    /// <summary>
    /// Signal fired when health component changes (current health, max health)
    /// Generic signal that can be used by any entity with HealthComponent
    /// </summary>
    public class HealthChangedSignal
    {
        public float CurrentHealth { get; set; }
        public float MaxHealth { get; set; }
    }
}


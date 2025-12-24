using UnityEngine;

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

    /// <summary>
    /// Signal fired when player invincibility state changes
    /// </summary>
    public class InvincibilityChangedSignal
    {
        public bool IsInvincible { get; set; }
    }

    /// <summary>
    /// Signal fired when a bullet is created
    /// </summary>
    public class BulletCreatedSignal
    {
        public GameEntity Entity { get; set; }
    }

    /// <summary>
    /// Signal fired when a bullet is shot
    /// </summary>
    public class BulletShotSignal
    {
    }

    /// <summary>
    /// Signal fired when a bullet is destroyed
    /// </summary>
    public class BulletDestroyedSignal
    {
        public GameEntity Entity { get; set; }
    }

    /// <summary>
    /// Signal fired when a laser is created
    /// </summary>
    public class LaserCreatedSignal
    {
        public GameEntity Entity { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Direction { get; set; }
        public float Duration { get; set; }
        public float Width { get; set; }
    }

    /// <summary>
    /// Signal fired when laser charges change
    /// </summary>
    public class LaserChargesChangedSignal
    {
        public int CurrentCharges { get; set; }
        public int MaxCharges { get; set; }
        public float RechargeTime { get; set; }
    }

    /// <summary>
    /// Signal fired when a laser is destroyed
    /// </summary>
    public class LaserDestroyedSignal
    {
        public GameEntity Entity { get; set; }
    }

    /// <summary>
    /// Signal fired when an enemy is destroyed
    /// </summary>
    public class EnemyDestroyedSignal
    {
        public GameEntity Entity { get; set; }
    }

    /// <summary>
    /// Signal fired when asteroid should fragment
    /// FragmentSize: 0 = Large, 1 = Medium, 2 = Small (AsteroidSize enum)
    /// </summary>
    public class AsteroidFragmentSignal
    {
        public Vector2 OriginalPosition { get; set; }
        public Vector2 OriginalVelocity { get; set; }
        public int FragmentSize { get; set; } // AsteroidSize enum value (0=Large, 1=Medium, 2=Small)
        public int FragmentCount { get; set; }
    }
}


using UnityEngine;

namespace Asteroids.Core.Entity.Components
{
    public class TransformChangedSignal
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Rotation { get; set; }
    }

    public class PhysicsChangedSignal
    {
        public float VelocityX { get; set; }
        public float VelocityY { get; set; }
        public float Speed { get; set; }
    }

    public class HealthChangedSignal
    {
        public float CurrentHealth { get; set; }
        public float MaxHealth { get; set; }
    }

    public class InvincibilityChangedSignal
    {
        public bool IsInvincible { get; set; }
    }

    public class BulletCreatedSignal
    {
        public GameEntity Entity { get; set; }
    }

    public class BulletShotSignal
    {
        public Vector2 Position { get; set; }
        public Vector2 Direction { get; set; }
    }

    public class BulletDestroyedSignal
    {
        public GameEntity Entity { get; set; }
    }

    public class EnemyDestroyedSignal
    {
        public GameEntity Entity { get; set; }
    }

    public class AsteroidFragmentSignal
    {
        public Vector2 OriginalPosition { get; set; }
        public Vector2 OriginalVelocity { get; set; }
        public int FragmentSize { get; set; }
        public int FragmentCount { get; set; }
    }

    public class LaserShotSignal
    {
        public Vector2 StartPosition { get; set; }
        public Vector2 Direction { get; set; }
    }

    public class LaserChargesChangedSignal
    {
        public int CurrentCharges { get; set; }
        public int MaxCharges { get; set; }
        public float RechargeProgress { get; set; }
    }

    public class LaserDeactivatedSignal
    {
    }

    public class ScoreChangedSignal
    {
        public int CurrentScore { get; set; }
        public int PointsAdded { get; set; }
    }

    public class GameStartedSignal
    {
    }

    public class GameOverSignal
    {
    }
}


using UnityEngine;

namespace Asteroids.Core.Entity.Components
{
    public class GameStartedSignal
    {
    }

    public class GameOverSignal
    {
    }

    public class EnemyDestroyedSignal
    {
        public GameEntity Entity { get; set; }
    }

    public class ScoreChangedSignal
    {
        public int CurrentScore { get; set; }
        public int PointsAdded { get; set; }
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

    public class BulletShotSignal
    {
        public Vector2 Position { get; set; }
        public Vector2 Direction { get; set; }
    }

    public class BulletDestroyedSignal
    {
        public GameEntity Entity { get; set; }
    }

    public class LaserShotSignal
    {
        public Vector2 StartPosition { get; set; }
        public Vector2 Direction { get; set; }
    }

    public class LaserDeactivatedSignal
    {
    }
}


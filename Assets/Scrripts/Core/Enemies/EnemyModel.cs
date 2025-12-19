using UnityEngine;

namespace Asteroids.Core.Enemies
{
    /// <summary>
    /// Base enemy model - contains common enemy data and state
    /// </summary>
    public class EnemyModel
    {
        public EnemyType Type { get; set; }
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public Vector2 Velocity { get; set; }
        public bool IsActive { get; set; }
        public float Health { get; set; }
        public float MaxHealth { get; set; }
        public float Speed { get; set; }

        public EnemyModel(EnemyType type)
        {
            Type = type;
            IsActive = true;
        }

        /// <summary>
        /// Take damage and check if enemy should be destroyed
        /// </summary>
        public void TakeDamage(float damage)
        {
            Health -= damage;
            if (Health <= 0)
            {
                IsActive = false;
            }
        }
    }
}


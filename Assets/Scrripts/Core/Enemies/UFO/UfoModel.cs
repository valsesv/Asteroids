using UnityEngine;

namespace Asteroids.Core.Enemies
{
    /// <summary>
    /// UFO model - contains UFO-specific data
    /// Speed is inherited from EnemyModel and will be loaded from JSON
    /// </summary>
    public class UfoModel : EnemyModel
    {
        public UfoModel() : base(EnemyType.Ufo)
        {
        }

        /// <summary>
        /// Calculate direction to pursue player
        /// </summary>
        public Vector2 GetPursuitDirection(Vector2 playerPosition)
        {
            Vector2 direction = playerPosition - Position;
            return direction.normalized;
        }
    }
}


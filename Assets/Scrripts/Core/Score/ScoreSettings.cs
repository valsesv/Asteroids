using System.Collections.Generic;
using Asteroids.Core.Enemies;

namespace Asteroids.Core.Score
{
    /// <summary>
    /// Settings for score rewards by enemy type
    /// Dictionary-based reward system as per requirements
    /// </summary>
    [System.Serializable]
    public class ScoreSettings
    {
        /// <summary>
        /// Dictionary of rewards by enemy type
        /// Key: EnemyType, Value: Points awarded
        /// Populated from JSON or default values
        /// </summary>
        public Dictionary<EnemyType, int> EnemyRewards { get; private set; }

        // JSON-serializable fields (Dictionary doesn't serialize well, so we use separate fields)
        public int AsteroidReward = 20;
        public int UfoReward = 200;
        public int FragmentReward = 50;

        public ScoreSettings()
        {
            InitializeRewards();
        }

        /// <summary>
        /// Initialize dictionary from JSON fields or default values
        /// Called after JSON deserialization
        /// </summary>
        public void InitializeRewards()
        {
            EnemyRewards = new Dictionary<EnemyType, int>
            {
                [EnemyType.Asteroid] = AsteroidReward,
                [EnemyType.Ufo] = UfoReward,
                [EnemyType.Fragment] = FragmentReward
            };
        }

        /// <summary>
        /// Get reward points for enemy type
        /// </summary>
        public int GetReward(EnemyType enemyType)
        {
            if (EnemyRewards == null)
            {
                InitializeRewards();
            }

            if (EnemyRewards.TryGetValue(enemyType, out int reward))
            {
                return reward;
            }
            return 0;
        }
    }
}


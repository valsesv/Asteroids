using System.Collections.Generic;
using Asteroids.Core.Enemies;

namespace Asteroids.Core.Score
{
    [System.Serializable]
    public class ScoreSettings
    {
        public Dictionary<EnemyType, int> EnemyRewards { get; private set; }

        public int AsteroidReward;
        public int UfoReward;
        public int FragmentReward;

        public ScoreSettings()
        {
            InitializeRewards();
        }
        public void InitializeRewards()
        {
            EnemyRewards = new Dictionary<EnemyType, int>
            {
                [EnemyType.Asteroid] = AsteroidReward,
                [EnemyType.Ufo] = UfoReward,
                [EnemyType.Fragment] = FragmentReward
            };
        }
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


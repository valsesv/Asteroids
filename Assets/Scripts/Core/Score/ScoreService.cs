using System.Collections.Generic;
using Zenject;
using Asteroids.Core.Enemies;
using Asteroids.Core.Entity.Components;
using System;

namespace Asteroids.Core.Score
{
    public class ScoreService : IInitializable, IDisposable
    {
        private readonly ScoreSettings _settings;
        private readonly SignalBus _signalBus;

        private Dictionary<EnemyType, int> _enemyRewards;
        private int _currentScore;

        public int CurrentScore => _currentScore;

        public ScoreService(ScoreSettings settings, SignalBus signalBus)
        {
            _settings = settings;
            _signalBus = signalBus;
        }

        public void Initialize()
        {
            InitializeRewards();
            _currentScore = 0;
            _signalBus.Subscribe<EnemyDestroyedSignal>(OnEnemyDestroyed);
            _signalBus.Subscribe<GameStartedSignal>(OnGameStarted);
        }

        public void Dispose()
        {
            _signalBus?.Unsubscribe<EnemyDestroyedSignal>(OnEnemyDestroyed);
            _signalBus?.Unsubscribe<GameStartedSignal>(OnGameStarted);
        }

        private void OnEnemyDestroyed(EnemyDestroyedSignal signal)
        {
            if (signal.Entity == null)
            {
                return;
            }

            var enemyComponent = signal.Entity.GetComponent<EnemyComponent>();
            if (enemyComponent == null)
            {
                return;
            }

            int reward = GetReward(enemyComponent.Type);

            if (reward > 0)
            {
                AddScore(reward);
            }
        }

        private void InitializeRewards()
        {
            _enemyRewards = new Dictionary<EnemyType, int>
            {
                [EnemyType.Asteroid] = _settings.AsteroidReward,
                [EnemyType.Ufo] = _settings.UfoReward,
                [EnemyType.Fragment] = _settings.FragmentReward
            };
        }

        private int GetReward(EnemyType enemyType)
        {
            if (_enemyRewards.TryGetValue(enemyType, out int reward))
            {
                return reward;
            }
            return 0;
        }

        private void AddScore(int points)
        {
            _currentScore += points;

            var scoreChangedSignal = new ScoreChangedSignal
            {
                CurrentScore = _currentScore,
                PointsAdded = points
            };
            _signalBus.Fire(scoreChangedSignal);
        }

        private void OnGameStarted(GameStartedSignal signal)
        {
            ResetScore();
        }

        public void ResetScore()
        {
            _currentScore = 0;
            var scoreChangedSignal = new ScoreChangedSignal
            {
                CurrentScore = _currentScore,
                PointsAdded = 0
            };
            _signalBus.Fire(scoreChangedSignal);
        }
    }
}


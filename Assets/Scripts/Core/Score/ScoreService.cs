using System;
using System.Collections.Generic;
using Zenject;
using Asteroids.Core.Enemies;
using Asteroids.Core.Entity;
using Asteroids.Core.Game;
using Asteroids.Presentation.Enemies;

namespace Asteroids.Core.Score
{
    public class ScoreService : IInitializable, IDisposable
    {
        public event Action<int> OnScoreChanged;

        private readonly ScoreSettings _settings;
        private readonly SignalBus _signalBus;
        private readonly EnemySpawner _enemySpawner;

        private Dictionary<EnemyType, int> _enemyRewards;
        public int CurrentScore { get; private set; }

        public ScoreService(ScoreSettings settings, SignalBus signalBus, EnemySpawner enemySpawner)
        {
            _settings = settings;
            _signalBus = signalBus;
            _enemySpawner = enemySpawner;
        }

        public void Initialize()
        {
            InitializeRewards();
            CurrentScore = 0;
            _enemySpawner.OnEnemyDestroyed += OnEnemyDestroyed;
            _signalBus.Subscribe<GameStartedSignal>(OnGameStarted);
        }

        public void Dispose()
        {
            _enemySpawner.OnEnemyDestroyed -= OnEnemyDestroyed;
            _signalBus?.Unsubscribe<GameStartedSignal>(OnGameStarted);
        }

        private void OnEnemyDestroyed(EnemyType enemyType)
        {
            int reward = GetReward(enemyType);

            AddScore(reward);
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
            CurrentScore += points;
            OnScoreChanged?.Invoke(CurrentScore);
        }

        private void OnGameStarted(GameStartedSignal _)
        {
            ResetScore();
        }

        public void ResetScore()
        {
            CurrentScore = 0;
            OnScoreChanged?.Invoke(CurrentScore);
        }
    }
}


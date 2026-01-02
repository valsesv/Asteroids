using Zenject;
using Asteroids.Core.Enemies;
using Asteroids.Core.Entity.Components;
using System;

namespace Asteroids.Core.Score
{
    /// <summary>
    /// Service for managing player score
    /// Handles score calculation based on enemy type rewards dictionary
    /// </summary>
    public class ScoreService : IInitializable, IDisposable
    {
        private readonly ScoreSettings _settings;
        private readonly SignalBus _signalBus;

        private int _currentScore;

        public int CurrentScore => _currentScore;

        public ScoreService(ScoreSettings settings, SignalBus signalBus)
        {
            _settings = settings;
            _signalBus = signalBus;
        }

        public void Initialize()
        {
            _currentScore = 0;
            _signalBus.Subscribe<EnemyDestroyedSignal>(OnEnemyDestroyed);
            _signalBus.Subscribe<GameStartedSignal>(OnGameStarted);
        }

        public void Dispose()
        {
            _signalBus?.Unsubscribe<EnemyDestroyedSignal>(OnEnemyDestroyed);
            _signalBus?.Unsubscribe<GameStartedSignal>(OnGameStarted);
        }

        /// <summary>
        /// Handle enemy destroyed - award points based on enemy type
        /// </summary>
        private void OnEnemyDestroyed(EnemyDestroyedSignal signal)
        {
            if (signal.Entity == null)
            {
                return;
            }

            // Get enemy component to determine type
            var enemyComponent = signal.Entity.GetComponent<EnemyComponent>();
            if (enemyComponent == null)
            {
                return;
            }

            // Get reward from dictionary based on enemy type
            int reward = _settings.GetReward(enemyComponent.Type);

            if (reward > 0)
            {
                AddScore(reward);
            }
        }

        /// <summary>
        /// Add points to current score
        /// </summary>
        private void AddScore(int points)
        {
            _currentScore += points;

            // Fire signal to notify UI and other systems
            var scoreChangedSignal = new ScoreChangedSignal
            {
                CurrentScore = _currentScore,
                PointsAdded = points
            };
            _signalBus.Fire(scoreChangedSignal);
        }

        /// <summary>
        /// Handle game started - reset score when new game begins
        /// </summary>
        private void OnGameStarted(GameStartedSignal signal)
        {
            ResetScore();
        }

        /// <summary>
        /// Reset score to zero
        /// </summary>
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


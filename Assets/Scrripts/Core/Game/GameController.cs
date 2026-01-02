using System;
using Zenject;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;
using Asteroids.Core.Player;
using Asteroids.Presentation.Enemies;

namespace Asteroids.Core.Game
{
    /// <summary>
    /// GameController - manages game state (start, playing, game over)
    /// Non-MonoBehaviour class that controls game flow
    /// </summary>
    public class GameController : IInitializable, IDisposable
    {
        private readonly SignalBus _signalBus;
        private readonly GameEntity _playerEntity;
        private readonly EnemySpawner _enemySpawner;
        private readonly StartPositionSettings _startPositionSettings;
        private readonly HealthSettings _healthSettings;

        public enum GameState
        {
            WaitingToStart,
            Playing,
            GameOver
        }

        private GameState _currentState = GameState.WaitingToStart;
        private bool _isPlayerDead = false;

        public GameState CurrentState => _currentState;
        public bool IsGameActive => _currentState == GameState.Playing;

        public GameController(
            SignalBus signalBus,
            GameEntity playerEntity,
            EnemySpawner enemySpawner,
            StartPositionSettings startPositionSettings,
            HealthSettings healthSettings)
        {
            _signalBus = signalBus;
            _playerEntity = playerEntity;
            _enemySpawner = enemySpawner;
            _startPositionSettings = startPositionSettings;
            _healthSettings = healthSettings;
        }

        public void Initialize()
        {
            // Start in waiting state - disable player control and enemy spawning
            SetGameState(GameState.WaitingToStart);

            // Subscribe to health changes to detect player death
            _signalBus.Subscribe<HealthChangedSignal>(OnHealthChanged);
        }

        public void Dispose()
        {
            _signalBus?.Unsubscribe<HealthChangedSignal>(OnHealthChanged);
        }

        /// <summary>
        /// Start the game - called when player clicks "Click to start"
        /// </summary>
        public void StartGame()
        {
            if (_currentState != GameState.WaitingToStart && _currentState != GameState.GameOver)
            {
                return; // Can only start from waiting or game over state
            }

            // Clear all existing enemies (return them to pool)
            _enemySpawner.ClearAllEnemies();

            // Reset player
            ResetPlayer();

            // Enable enemy spawning
            _enemySpawner.SetSpawningEnabled(true);

            // Set game state to playing
            SetGameState(GameState.Playing);

            // Fire game started signal
            _signalBus.Fire<GameStartedSignal>();
        }

        /// <summary>
        /// Handle game over - called when player loses all lives
        /// </summary>
        private void OnGameOver()
        {
            if (_currentState != GameState.Playing)
            {
                return; // Can only go to game over from playing state
            }

            // Disable player control
            var shipComponent = _playerEntity.GetComponent<ShipComponent>();
            if (shipComponent != null)
            {
                shipComponent.CanControl = false;
            }

            // Stop enemy spawning
            _enemySpawner.SetSpawningEnabled(false);

            // Stop all enemies from moving (set their velocity to zero)
            StopAllEnemies();

            // Set game state to game over
            SetGameState(GameState.GameOver);

            // Fire game over signal
            _signalBus.Fire<GameOverSignal>();
        }

        /// <summary>
        /// Handle health changes - detect when player dies
        /// </summary>
        private void OnHealthChanged(HealthChangedSignal signal)
        {
            // Check if this is the player's health (we can identify by checking if health reached 0)
            if (signal.CurrentHealth <= 0f && !_isPlayerDead)
            {
                _isPlayerDead = true;
                OnGameOver();
            }
        }

        /// <summary>
        /// Reset player to initial state
        /// </summary>
        private void ResetPlayer()
        {
            _isPlayerDead = false;

            // Reset position
            var transformComponent = _playerEntity.GetComponent<TransformComponent>();
            if (transformComponent != null)
            {
                transformComponent.SetPosition(_startPositionSettings.Position);
                transformComponent.SetRotation(_startPositionSettings.Rotation);
            }

            // Reset physics (velocity to zero)
            var physicsComponent = _playerEntity.GetComponent<PhysicsComponent>();
            if (physicsComponent != null)
            {
                physicsComponent.SetVelocity(UnityEngine.Vector2.zero);
            }

            // Reset health
            var healthComponent = _playerEntity.GetComponent<HealthComponent>();
            if (healthComponent != null)
            {
                healthComponent.ResetHealth();
            }

            // Enable player control
            var shipComponent = _playerEntity.GetComponent<ShipComponent>();
            if (shipComponent != null)
            {
                shipComponent.CanControl = true;
            }
        }

        /// <summary>
        /// Stop all enemies from moving by setting their velocity to zero
        /// </summary>
        private void StopAllEnemies()
        {
            foreach (var enemy in _enemySpawner.ActiveEnemies)
            {
                if (enemy?.Entity != null)
                {
                    var physicsComponent = enemy.Entity.GetComponent<PhysicsComponent>();
                    if (physicsComponent != null)
                    {
                        physicsComponent.SetVelocity(UnityEngine.Vector2.zero);
                    }
                }
            }
        }

        /// <summary>
        /// Set game state and update related flags
        /// </summary>
        private void SetGameState(GameState newState)
        {
            _currentState = newState;

            // Update player control based on state
            var shipComponent = _playerEntity.GetComponent<ShipComponent>();
            if (shipComponent != null)
            {
                shipComponent.CanControl = (newState == GameState.Playing);
            }
        }
    }
}


using System;
using Zenject;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;
using Asteroids.Core.Player;
using Asteroids.Presentation.Enemies;

namespace Asteroids.Core.Game
{
    public class GameController : IInitializable, IDisposable
    {
        private readonly SignalBus _signalBus;
        private readonly EnemySpawner _enemySpawner;
        private readonly StartPositionSettings _startPositionSettings;

        private HealthComponent _healthComponent;
        private ShipComponent _shipComponent;
        private TransformComponent _transformComponent;
        private PhysicsComponent _physicsComponent;
        private GameState _currentState = GameState.WaitingToStart;

        public GameState CurrentState => _currentState;
        public bool IsGameActive => _currentState == GameState.Playing;

        public GameController(
            SignalBus signalBus,
            GameEntity playerEntity,
            EnemySpawner enemySpawner,
            StartPositionSettings startPositionSettings)
        {
            _signalBus = signalBus;
            _enemySpawner = enemySpawner;
            _startPositionSettings = startPositionSettings;
            _healthComponent = playerEntity.GetComponent<HealthComponent>();
            _shipComponent = playerEntity.GetComponent<ShipComponent>();
            _transformComponent = playerEntity.GetComponent<TransformComponent>();
            _physicsComponent = playerEntity.GetComponent<PhysicsComponent>();
        }

        public void Initialize()
        {
            SetGameState(GameState.WaitingToStart);
            _signalBus.Subscribe<GameOverSignal>(OnGameOver);

            _healthComponent.OnDeath += OnPlayerDeath;
        }

        public void Dispose()
        {
            _signalBus?.Unsubscribe<GameOverSignal>(OnGameOver);

            _healthComponent.OnDeath -= OnPlayerDeath;
        }

        public void StartGame()
        {
            if (_currentState == GameState.Playing)
            {
                return;
            }

            _enemySpawner.ClearAllEnemies();
            _enemySpawner.SetSpawningEnabled(true);
            ResetPlayer();
            SetGameState(GameState.Playing);
            _signalBus.Fire<GameStartedSignal>();
        }

        private void OnPlayerDeath()
        {
            _signalBus?.Fire<GameOverSignal>();
        }

        private void OnGameOver(GameOverSignal _)
        {
            if (_currentState != GameState.Playing)
            {
                return;
            }

            _shipComponent.CanControl = false;

            StopAllEnemies();
            SetGameState(GameState.GameOver);
        }

        private void ResetPlayer()
        {
            _transformComponent.SetPosition(_startPositionSettings.Position);
            _transformComponent.SetRotation(_startPositionSettings.Rotation);
            _physicsComponent.SetVelocity(UnityEngine.Vector2.zero);
            _healthComponent.ResetHealth();
            _shipComponent.CanControl = true;
        }

        private void StopAllEnemies()
        {
            _enemySpawner.SetSpawningEnabled(false);
            foreach (var enemy in _enemySpawner.ActiveEnemies)
            {
                if (enemy?.Entity == null)
                {
                    continue;
                }
                var physicsComponent = enemy.Entity.GetComponent<PhysicsComponent>();
                physicsComponent.SetVelocity(UnityEngine.Vector2.zero);
            }
        }

        private void SetGameState(GameState newState)
        {
            _currentState = newState;

            _shipComponent.CanControl = newState == GameState.Playing;
        }
    }
}


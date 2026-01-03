using System;
using Zenject;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;
using Asteroids.Core.Player;
using Asteroids.Presentation.Enemies;

namespace Asteroids.Core.Game
{
    public partial class GameController : IInitializable, IDisposable
    {
        private readonly SignalBus _signalBus;
        private readonly GameEntity _playerEntity;
        private readonly EnemySpawner _enemySpawner;
        private readonly StartPositionSettings _startPositionSettings;

        private GameState _currentState = GameState.WaitingToStart;

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
        }

        public void Initialize()
        {
            SetGameState(GameState.WaitingToStart);
            _signalBus.Subscribe<GameOverSignal>(OnGameOver);
        }

        public void Dispose()
        {
            _signalBus?.Unsubscribe<GameOverSignal>(OnGameOver);
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

        private void OnGameOver(GameOverSignal _)
        {
            if (_currentState != GameState.Playing)
            {
                return;
            }

            var shipComponent = _playerEntity.GetComponent<ShipComponent>();
            shipComponent.CanControl = false;

            StopAllEnemies();
            SetGameState(GameState.GameOver);
        }

        private void ResetPlayer()
        {
            var transformComponent = _playerEntity.GetComponent<TransformComponent>();
            transformComponent.SetPosition(_startPositionSettings.Position);
            transformComponent.SetRotation(_startPositionSettings.Rotation);

            var physicsComponent = _playerEntity.GetComponent<PhysicsComponent>();
            physicsComponent.SetVelocity(UnityEngine.Vector2.zero);

            var healthComponent = _playerEntity.GetComponent<HealthComponent>();
            healthComponent.ResetHealth();

            var shipComponent = _playerEntity.GetComponent<ShipComponent>();
            shipComponent.CanControl = true;
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

            var shipComponent = _playerEntity.GetComponent<ShipComponent>();
            shipComponent.CanControl = newState == GameState.Playing;
        }
    }
}


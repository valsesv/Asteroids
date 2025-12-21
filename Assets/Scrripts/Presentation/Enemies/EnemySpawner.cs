using UnityEngine;
using Zenject;
using Asteroids.Core.Entity;
using Asteroids.Core.Enemies;

namespace Asteroids.Presentation.Enemies
{
    /// <summary>
    /// Simple enemy spawner that creates enemies at random positions outside screen bounds
    /// </summary>
    public class EnemySpawner : MonoBehaviour, IInitializable, ITickable
    {
        [SerializeField] private GameObject _asteroidPrefab;
        [SerializeField] private GameObject _ufoPrefab;
        [SerializeField] private float _spawnInterval = 3f;
        [SerializeField] private float _spawnDistance = 1.2f; // Distance outside screen bounds

        private float _lastSpawnTime;
        private ScreenBounds _screenBounds;
        private DiContainer _container;
        private EnemySettings _enemySettings;
        private int _currentEnemyCount = 0;

        [Inject]
        public void Construct(ScreenBounds screenBounds, DiContainer container, EnemySettings enemySettings)
        {
            _screenBounds = screenBounds;
            _container = container;
            _enemySettings = enemySettings;
        }

        public void Initialize()
        {
            _lastSpawnTime = Time.time;
        }

        public void Tick()
        {
            // Check if we can spawn more enemies (don't exceed max count)
            if (_currentEnemyCount >= _enemySettings.MaxEnemiesOnMap)
            {
                return;
            }

            if (Time.time - _lastSpawnTime >= _spawnInterval)
            {
                SpawnRandomEnemy();
                _lastSpawnTime = Time.time;
                _currentEnemyCount++;
            }
        }

        private void SpawnRandomEnemy()
        {
            // Randomly choose enemy type
            bool spawnAsteroid = Random.Range(0, 2) == 0;
            GameObject prefab = spawnAsteroid ? _asteroidPrefab : _ufoPrefab;

            if (prefab == null)
            {
                Debug.LogWarning($"Enemy prefab is not assigned in EnemySpawner!");
                return;
            }

            // Get random spawn position outside screen bounds
            Vector2 spawnPosition = GetRandomSpawnPosition();

            // Instantiate enemy with GameObjectContext
            GameObject enemy = _container.InstantiatePrefab(prefab);
            enemy.transform.position = new Vector3(spawnPosition.x, spawnPosition.y, 0f);

            // Note: Enemy count is incremented in Tick() method
            // To properly track enemy destruction, you would need to:
            // 1. Subscribe to enemy death signals
            // 2. Decrement _currentEnemyCount when enemy is destroyed
            // For now, count is incremented on spawn and will be managed later
        }

        private Vector2 GetRandomSpawnPosition()
        {
            // Randomly choose which side of screen to spawn from
            int side = Random.Range(0, 4);
            float x, y;

            switch (side)
            {
                case 0: // Top
                    x = Random.Range(_screenBounds.Left, _screenBounds.Right);
                    y = _screenBounds.Top + _spawnDistance;
                    break;
                case 1: // Bottom
                    x = Random.Range(_screenBounds.Left, _screenBounds.Right);
                    y = _screenBounds.Bottom - _spawnDistance;
                    break;
                case 2: // Left
                    x = _screenBounds.Left - _spawnDistance;
                    y = Random.Range(_screenBounds.Bottom, _screenBounds.Top);
                    break;
                default: // Right
                    x = _screenBounds.Right + _spawnDistance;
                    y = Random.Range(_screenBounds.Bottom, _screenBounds.Top);
                    break;
            }

            return new Vector2(x, y);
        }
    }
}


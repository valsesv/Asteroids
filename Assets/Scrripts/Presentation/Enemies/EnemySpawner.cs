using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Asteroids.Core.Entity;
using Asteroids.Core.Enemies;

namespace Asteroids.Presentation.Enemies
{
    /// <summary>
    /// Enemy spawner that uses ObjectPool and factories to spawn enemies outside screen bounds
    /// </summary>
    public class EnemySpawner : MonoBehaviour, IInitializable, ITickable
    {
        [SerializeField] private Transform _asteroidParent;
        [SerializeField] private Transform _ufoParent;
        [SerializeField] private Transform _fragmentParent;
        [SerializeField] private GameObject _asteroidPrefab;
        [SerializeField] private GameObject _ufoPrefab;
        [SerializeField] private GameObject _fragmentPrefab;

        private float _lastSpawnTime;
        private ScreenBounds _screenBounds;
        private DiContainer _container;
        private EnemySettings _enemySettings;

        // Object pools for enemies
        private ObjectPool<AsteroidView> _asteroidPool;
        private ObjectPool<UfoView> _ufoPool;
        private ObjectPool<FragmentView> _fragmentPool;

        // Factories for creating enemies
        private EnemyViewFactory<AsteroidView> _asteroidFactory;
        private EnemyViewFactory<UfoView> _ufoFactory;
        private EnemyViewFactory<FragmentView> _fragmentFactory;

        // List to store all active enemies in one place
        private List<EnemyView> _activeEnemies = new List<EnemyView>();

        /// <summary>
        /// Get all active enemies (similar to List<T>)
        /// </summary>
        public List<EnemyView> ActiveEnemies => _activeEnemies;

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

            // Create factories
            _asteroidFactory = new EnemyViewFactory<AsteroidView>(_container, _asteroidPrefab);
            _ufoFactory = new EnemyViewFactory<UfoView>(_container, _ufoPrefab);
            _fragmentFactory = new EnemyViewFactory<FragmentView>(_container, _fragmentPrefab);

            // Create object pools with factories
            // Factory will be called with position when Get() is called, but we need to set position after getting from pool
            _asteroidPool = new ObjectPool<AsteroidView>(() => _asteroidFactory.Create(Vector2.zero), _asteroidParent);
            _ufoPool = new ObjectPool<UfoView>(() => _ufoFactory.Create(Vector2.zero), _ufoParent);
            _fragmentPool = new ObjectPool<FragmentView>(() => _fragmentFactory.Create(Vector2.zero), _fragmentParent);
        }

        public void Tick()
        {
            // Check if we can spawn more enemies (don't exceed max count)
            if (_activeEnemies.Count >= _enemySettings.MaxEnemiesOnMap)
            {
                return;
            }

            if (Time.time - _lastSpawnTime >= _enemySettings.SpawnInterval)
            {
                SpawnRandomEnemy();
                _lastSpawnTime = Time.time;
            }
        }

        private void SpawnRandomEnemy()
        {
            // Get random spawn position outside screen bounds
            Vector2 spawnPosition = GetRandomSpawnPosition();

            // Randomly choose enemy type based on spawn weights
            float totalWeight = _enemySettings.AsteroidSpawnWeight + _enemySettings.UfoSpawnWeight;
            float randomValue = Random.Range(0f, totalWeight);
            bool spawnAsteroid = randomValue < _enemySettings.AsteroidSpawnWeight;

            EnemyView enemy;
            if (spawnAsteroid)
            {
                var asteroid = _asteroidPool.Get();
                enemy = asteroid;

                // Set spawn position first (updates both Unity transform and TransformComponent)
                enemy.SetSpawnPosition(spawnPosition);

                // Calculate direction to random point inside game area
                Vector2 targetPoint = GetRandomPointInsideGameArea();
                Vector2 direction = (targetPoint - spawnPosition).normalized;
                asteroid.SetDirection(direction);
            }
            else
            {
                var ufo = _ufoPool.Get();
                enemy = ufo;

                // Set spawn position (updates both Unity transform and TransformComponent)
                enemy.SetSpawnPosition(spawnPosition);
            }

            // Add to active enemies list
            _activeEnemies.Add(enemy);
        }

        private Vector2 GetRandomPointInsideGameArea()
        {
            float x = Random.Range(_screenBounds.Left, _screenBounds.Right);
            float y = Random.Range(_screenBounds.Bottom, _screenBounds.Top);
            return new Vector2(x, y);
        }

        /// <summary>
        /// Return enemy to pool (call this when enemy is destroyed)
        /// </summary>
        public void ReturnEnemy(EnemyView enemy)
        {
            if (enemy == null)
            {
                return;
            }

            _activeEnemies.Remove(enemy);

            if (enemy is AsteroidView asteroid)
            {
                _asteroidPool.Return(asteroid);
            }
            else if (enemy is UfoView ufo)
            {
                _ufoPool.Return(ufo);
            }
            else if (enemy is FragmentView fragment)
            {
                _fragmentPool.Return(fragment);
            }
        }

        /// <summary>
        /// Fragment an asteroid into Fragment enemies
        /// </summary>
        public void FragmentAsteroid(AsteroidView originalAsteroid, Vector2 position, Vector2 originalVelocity, AsteroidComponent asteroidComponent)
        {
            if (originalAsteroid == null || asteroidComponent == null)
            {
                return;
            }

            // Return original asteroid to pool
            ReturnEnemy(originalAsteroid);

            // Get fragment count
            int fragmentCount = asteroidComponent.GetFragmentCount();

            // Spawn Fragment enemies
            for (int i = 0; i < fragmentCount; i++)
            {
                // Get fragment from pool
                var fragment = _fragmentPool.Get();

                // Set spawn position
                fragment.SetSpawnPosition(position);

                // Calculate fragment direction (spread out from backward direction of original velocity)
                float angleOffset = (i - fragmentCount / 2f + 0.5f) * 45f * Mathf.Deg2Rad; // Spread fragments in 45-degree increments
                Vector2 backwardDirection = -originalVelocity.normalized; // Move in opposite direction
                Vector2 fragmentDirection = RotateVector(backwardDirection, angleOffset);

                // Set fragment direction (this will set the velocity via FragmentMovement)
                fragment.SetDirection(fragmentDirection);

                // Add to active enemies list
                _activeEnemies.Add(fragment);
            }
        }

        /// <summary>
        /// Rotate a vector by an angle (in radians)
        /// </summary>
        private Vector2 RotateVector(Vector2 vector, float angle)
        {
            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);
            return new Vector2(
                vector.x * cos - vector.y * sin,
                vector.x * sin + vector.y * cos
            );
        }

        private Vector2 GetRandomSpawnPosition()
        {
            // Randomly choose which side of screen to spawn from
            int side = Random.Range(0, 4);
            float x, y;

            // Calculate safe spawn distance to ensure enemies spawn completely outside screen
            float safeDistance = _enemySettings.SpawnDistance;

            switch (side)
            {
                case 0: // Top - spawn above screen
                    x = Random.Range(_screenBounds.Left - safeDistance, _screenBounds.Right + safeDistance);
                    y = _screenBounds.Top + safeDistance;
                    break;
                case 1: // Bottom - spawn below screen
                    x = Random.Range(_screenBounds.Left - safeDistance, _screenBounds.Right + safeDistance);
                    y = _screenBounds.Bottom - safeDistance;
                    break;
                case 2: // Left - spawn to the left of screen
                    x = _screenBounds.Left - safeDistance;
                    y = Random.Range(_screenBounds.Bottom - safeDistance, _screenBounds.Top + safeDistance);
                    break;
                default: // Right - spawn to the right of screen
                    x = _screenBounds.Right + safeDistance;
                    y = Random.Range(_screenBounds.Bottom - safeDistance, _screenBounds.Top + safeDistance);
                    break;
            }

            return new Vector2(x, y);
        }
    }
}


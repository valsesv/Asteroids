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
        [SerializeField] private GameObject _asteroidPrefab;
        [SerializeField] private GameObject _ufoPrefab;
        [SerializeField] private float _spawnInterval = 3f;
        [SerializeField] private float _spawnDistance = 1.2f; // Distance outside screen bounds

        private float _lastSpawnTime;
        private ScreenBounds _screenBounds;
        private DiContainer _container;
        private EnemySettings _enemySettings;

        // Object pools for enemies
        private ObjectPool<AsteroidView> _asteroidPool;
        private ObjectPool<UfoView> _ufoPool;

        // Factories for creating enemies
        private EnemyViewFactory<AsteroidView> _asteroidFactory;
        private EnemyViewFactory<UfoView> _ufoFactory;

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

            // Create object pools with factories
            // Factory will be called with position when Get() is called, but we need to set position after getting from pool
            _asteroidPool = new ObjectPool<AsteroidView>(() => _asteroidFactory.Create(Vector2.zero), _asteroidParent);
            _ufoPool = new ObjectPool<UfoView>(() => _ufoFactory.Create(Vector2.zero), _ufoParent);
        }

        public void Tick()
        {
            // Check if we can spawn more enemies (don't exceed max count)
            if (_activeEnemies.Count >= _enemySettings.MaxEnemiesOnMap)
            {
                return;
            }

            if (Time.time - _lastSpawnTime >= _spawnInterval)
            {
                SpawnRandomEnemy();
                _lastSpawnTime = Time.time;
            }
        }

        private void SpawnRandomEnemy()
        {
            // Get random spawn position outside screen bounds
            Vector2 spawnPosition = GetRandomSpawnPosition();

            // Randomly choose enemy type
            bool spawnAsteroid = Random.Range(0, 2) == 0;

            EnemyView enemy;
            if (spawnAsteroid)
            {
                var asteroid = _asteroidPool.Get();
                asteroid.transform.position = new Vector3(spawnPosition.x, spawnPosition.y, 0f);
                enemy = asteroid;
            }
            else
            {
                var ufo = _ufoPool.Get();
                ufo.transform.position = new Vector3(spawnPosition.x, spawnPosition.y, 0f);
                enemy = ufo;
            }

            // Add to active enemies list
            _activeEnemies.Add(enemy);
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


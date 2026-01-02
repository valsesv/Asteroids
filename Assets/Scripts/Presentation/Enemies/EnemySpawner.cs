using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Asteroids.Core.Entity;
using Asteroids.Core.Enemies;

namespace Asteroids.Presentation.Enemies
{
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

        private ObjectPool<AsteroidView> _asteroidPool;
        private ObjectPool<UfoView> _ufoPool;
        private ObjectPool<FragmentView> _fragmentPool;

        private EnemyViewFactory<AsteroidView> _asteroidFactory;
        private EnemyViewFactory<UfoView> _ufoFactory;
        private EnemyViewFactory<FragmentView> _fragmentFactory;

        private List<EnemyView> _activeEnemies = new List<EnemyView>();
        private bool _isSpawningEnabled = false;

        public List<EnemyView> ActiveEnemies => _activeEnemies;

        public void SetSpawningEnabled(bool enabled)
        {
            _isSpawningEnabled = enabled;
        }

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

            _asteroidFactory = new EnemyViewFactory<AsteroidView>(_container, _asteroidPrefab);
            _ufoFactory = new EnemyViewFactory<UfoView>(_container, _ufoPrefab);
            _fragmentFactory = new EnemyViewFactory<FragmentView>(_container, _fragmentPrefab);

            _asteroidPool = new ObjectPool<AsteroidView>(() => _asteroidFactory.Create(Vector2.zero), _asteroidParent);
            _ufoPool = new ObjectPool<UfoView>(() => _ufoFactory.Create(Vector2.zero), _ufoParent);
            _fragmentPool = new ObjectPool<FragmentView>(() => _fragmentFactory.Create(Vector2.zero), _fragmentParent);
        }

        public void Tick()
        {
            if (!_isSpawningEnabled)
            {
                return;
            }

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
            Vector2 spawnPosition = GetRandomSpawnPosition();

            float totalWeight = _enemySettings.AsteroidSpawnWeight + _enemySettings.UfoSpawnWeight;
            float randomValue = Random.Range(0f, totalWeight);
            bool spawnAsteroid = randomValue < _enemySettings.AsteroidSpawnWeight;

            EnemyView enemy;
            if (spawnAsteroid)
            {
                var asteroid = _asteroidPool.Get();
                enemy = asteroid;

                enemy.SetSpawnPosition(spawnPosition);

                Vector2 targetPoint = GetRandomPointInsideGameArea();
                Vector2 direction = (targetPoint - spawnPosition).normalized;
                asteroid.SetDirection(direction);
            }
            else
            {
                var ufo = _ufoPool.Get();
                enemy = ufo;

                enemy.SetSpawnPosition(spawnPosition);
            }

            _activeEnemies.Add(enemy);
        }

        private Vector2 GetRandomPointInsideGameArea()
        {
            float x = Random.Range(_screenBounds.Left, _screenBounds.Right);
            float y = Random.Range(_screenBounds.Bottom, _screenBounds.Top);
            return new Vector2(x, y);
        }

        public void ReturnEnemy(EnemyView enemy)
        {
            if (enemy == null)
            {
                return;
            }

            _activeEnemies.Remove(enemy);

            switch (enemy)
            {
                case AsteroidView asteroid:
                    _asteroidPool.Return(asteroid);
                    break;
                case UfoView ufo:
                    _ufoPool.Return(ufo);
                    break;
                case FragmentView fragment:
                    _fragmentPool.Return(fragment);
                    break;
            }
        }

        public void ClearAllEnemies()
        {
            var enemiesToReturn = new List<EnemyView>(_activeEnemies);

            foreach (var enemy in enemiesToReturn)
            {
                ReturnEnemy(enemy);
            }
        }

        public void FragmentAsteroid(AsteroidView originalAsteroid, Vector2 position, Vector2 originalVelocity, AsteroidComponent asteroidComponent)
        {
            if (originalAsteroid == null || asteroidComponent == null)
            {
                return;
            }

            ReturnEnemy(originalAsteroid);

            int fragmentCount = _enemySettings.AsteroidFragmentCount;

            for (int i = 0; i < fragmentCount; i++)
            {
                var fragment = _fragmentPool.Get();

                fragment.SetSpawnPosition(position);

                float angleOffset = (i - fragmentCount / 2f + 0.5f) * 45f * Mathf.Deg2Rad;
                Vector2 backwardDirection = -originalVelocity.normalized;
                Vector2 fragmentDirection = RotateVector(backwardDirection, angleOffset);

                fragment.SetDirection(fragmentDirection);

                _activeEnemies.Add(fragment);
            }
        }

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
            int side = Random.Range(0, 4);
            float x, y;

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


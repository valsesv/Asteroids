using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Asteroids.Core.Entity;
using Asteroids.Core.Enemies;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace Asteroids.Presentation.Enemies
{
    public class EnemySpawner : MonoBehaviour, IInitializable, ITickable
    {
        public List<EnemyPresentation> ActiveEnemies => _activeEnemies;
        public event Action<GameEntity> OnEnemyDestroyed;

        [SerializeField] private Transform _asteroidParent;
        [SerializeField] private Transform _ufoParent;
        [SerializeField] private Transform _fragmentParent;
        [SerializeField] private GameObject _asteroidPrefab;
        [SerializeField] private GameObject _ufoPrefab;
        [SerializeField] private GameObject _fragmentPrefab;

        private ScreenBounds _screenBounds;
        private DiContainer _container;
        private EnemySettings _enemySettings;

        private ObjectPool<AsteroidPresentation> _asteroidPool;
        private ObjectPool<UfoPresentation> _ufoPool;
        private ObjectPool<FragmentPresentation> _fragmentPool;
        private EnemyPresentationFactory<AsteroidPresentation> _asteroidFactory;
        private EnemyPresentationFactory<UfoPresentation> _ufoFactory;
        private EnemyPresentationFactory<FragmentPresentation> _fragmentFactory;
        private List<EnemyPresentation> _activeEnemies = new List<EnemyPresentation>();
        private float _lastSpawnTime;
        private bool _isSpawningEnabled = false;

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
            Assert.IsNotNull(_asteroidParent, "AsteroidParent is not assigned in EnemySpawner!");
            Assert.IsNotNull(_ufoParent, "UfoParent is not assigned in EnemySpawner!");
            Assert.IsNotNull(_fragmentParent, "FragmentParent is not assigned in EnemySpawner!");
            Assert.IsNotNull(_asteroidPrefab, "AsteroidPrefab is not assigned in EnemySpawner!");
            Assert.IsNotNull(_ufoPrefab, "UfoPrefab is not assigned in EnemySpawner!");
            Assert.IsNotNull(_fragmentPrefab, "FragmentPrefab is not assigned in EnemySpawner!");

            _lastSpawnTime = Time.time;

            _asteroidFactory = new EnemyPresentationFactory<AsteroidPresentation>(_container, _asteroidPrefab);
            _ufoFactory = new EnemyPresentationFactory<UfoPresentation>(_container, _ufoPrefab);
            _fragmentFactory = new EnemyPresentationFactory<FragmentPresentation>(_container, _fragmentPrefab);

            _asteroidPool = new ObjectPool<AsteroidPresentation>(() => _asteroidFactory.Create(Vector2.zero), _asteroidParent);
            _ufoPool = new ObjectPool<UfoPresentation>(() => _ufoFactory.Create(Vector2.zero), _ufoParent);
            _fragmentPool = new ObjectPool<FragmentPresentation>(() => _fragmentFactory.Create(Vector2.zero), _fragmentParent);
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

            EnemyPresentation enemy;
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

        public void ReturnEnemy(EnemyPresentation enemy)
        {
            if (enemy == null)
            {
                return;
            }

            if (enemy.Entity != null)
            {
                OnEnemyDestroyed?.Invoke(enemy.Entity);
            }

            _activeEnemies.Remove(enemy);

            switch (enemy)
            {
                case AsteroidPresentation asteroid:
                    _asteroidPool.Return(asteroid);
                    break;
                case UfoPresentation ufo:
                    _ufoPool.Return(ufo);
                    break;
                case FragmentPresentation fragment:
                    _fragmentPool.Return(fragment);
                    break;
            }
        }

        public void ClearAllEnemies()
        {
            var enemiesToReturn = new List<EnemyPresentation>(_activeEnemies);

            foreach (var enemy in enemiesToReturn)
            {
                ReturnEnemy(enemy);
            }
        }

        public void FragmentAsteroid(AsteroidPresentation originalAsteroid, Vector2 position, Vector2 originalVelocity, AsteroidComponent asteroidComponent)
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


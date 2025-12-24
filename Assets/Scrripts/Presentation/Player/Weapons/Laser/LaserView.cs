using System;
using UnityEngine;
using Zenject;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;
using Asteroids.Core.Player;
using Cysharp.Threading.Tasks;

namespace Asteroids.Presentation.Player
{
    /// <summary>
    /// Laser view - MonoBehaviour that represents a laser beam in the scene
    /// </summary>
    public class LaserView : MonoBehaviour, IInitializable, IDisposable
    {
        public GameEntity Entity { get; private set; }

        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private float _laserLength = 2000f;

        private LaserComponent _laserComponent;
        private TransformComponent _transformComponent;
        private LaserCollisionHandler _collisionHandler;

        [Inject] private SignalBus _signalBus;
        [Inject] private Enemies.EnemySpawner _enemySpawner;

        [Inject]
        public void Construct(GameEntity entity)
        {
            Entity = entity;
        }

        public void Initialize()
        {
            _laserComponent = Entity?.GetComponent<LaserComponent>();
            _transformComponent = Entity?.GetComponent<TransformComponent>();
            _collisionHandler = Entity?.GetComponent<LaserCollisionHandler>();

            if (_lineRenderer == null)
            {
                _lineRenderer = GetComponent<LineRenderer>();
            }

            if (_lineRenderer == null)
            {
                _lineRenderer = gameObject.AddComponent<LineRenderer>();
                _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
                _lineRenderer.startColor = Color.red;
                _lineRenderer.endColor = Color.red;
                _lineRenderer.startWidth = _laserComponent?.Width ?? 0.2f;
                _lineRenderer.endWidth = _laserComponent?.Width ?? 0.2f;
            }

            // Start laser effect
            StartLaserEffect().Forget();
        }

        public void Dispose()
        {
            // Cleanup
        }

        private async UniTaskVoid StartLaserEffect()
        {
            if (_laserComponent == null || _transformComponent == null)
            {
                return;
            }

            float elapsed = 0f;
            float duration = _laserComponent.Duration;

            while (elapsed < duration)
            {
                UpdateLaserBeam();
                CheckCollisions();

                elapsed += Time.deltaTime;
                await UniTask.Yield();
            }

            // Destroy laser
            _signalBus?.Fire(new LaserDestroyedSignal { Entity = Entity });
        }

        private void UpdateLaserBeam()
        {
            if (_transformComponent == null || _lineRenderer == null)
            {
                return;
            }

            Vector2 position = _transformComponent.Position;
            float angle = _transformComponent.Rotation * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            Vector2 startPoint = position;
            Vector2 endPoint = position + direction * _laserLength;

            _lineRenderer.SetPosition(0, new Vector3(startPoint.x, startPoint.y, 0f));
            _lineRenderer.SetPosition(1, new Vector3(endPoint.x, endPoint.y, 0f));
        }

        private void CheckCollisions()
        {
            if (_collisionHandler == null || _enemySpawner == null)
            {
                return;
            }

            // Get all active enemies
            var enemies = _enemySpawner.ActiveEnemies;
            if (enemies == null || enemies.Count == 0)
            {
                return;
            }

            // Convert EnemyView list to GameEntity list
            var enemyEntities = new System.Collections.Generic.List<GameEntity>();
            foreach (var enemyView in enemies)
            {
                if (enemyView != null && enemyView.Entity != null)
                {
                    enemyEntities.Add(enemyView.Entity);
                }
            }

            _collisionHandler.CheckCollisions(enemyEntities);
        }
    }
}


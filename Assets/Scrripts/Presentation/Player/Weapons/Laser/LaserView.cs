using System;
using UnityEngine;
using Zenject;
using Asteroids.Core.Entity.Components;
using Asteroids.Presentation.Enemies;
using Asteroids.Core.Player;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace Asteroids.Presentation.Player
{
    /// <summary>
    /// Laser view - MonoBehaviour that represents a laser beam in the scene
    /// Uses LineRenderer for visualization and custom physics for collision detection
    /// </summary>
    public class LaserView : MonoBehaviour, IInitializable, IDisposable
    {
        [SerializeField] private LineRenderer _lineRenderer;

        private SignalBus _signalBus;
        private EnemySpawner _enemySpawner;
        private LaserSettings _laserSettings;
        private CancellationTokenSource _cancellationTokenSource;

        private Vector2 _startPosition;
        private Vector2 _direction;
        private bool _isActive;

        [Inject]
        public void Construct(SignalBus signalBus, EnemySpawner enemySpawner, LaserSettings laserSettings)
        {
            _signalBus = signalBus;
            _enemySpawner = enemySpawner;
            _laserSettings = laserSettings;
        }

        public void Initialize()
        {
            _signalBus.Subscribe<LaserShotSignal>(OnLaserShot);
            _signalBus.Subscribe<LaserDeactivatedSignal>(OnLaserDeactivated);

            _lineRenderer.startWidth = _laserSettings.Width;
            _lineRenderer.endWidth = _laserSettings.Width;
            _lineRenderer.positionCount = 2;
            _lineRenderer.useWorldSpace = true;
            _lineRenderer.enabled = false;
        }

        public void Dispose()
        {
            _signalBus?.Unsubscribe<LaserShotSignal>(OnLaserShot);
            _signalBus?.Unsubscribe<LaserDeactivatedSignal>(OnLaserDeactivated);
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }

        private void OnLaserShot(LaserShotSignal signal)
        {
            _startPosition = signal.StartPosition;
            _direction = signal.Direction;
            _isActive = true;

            // Update LineRenderer
            UpdateLaserVisualization();
            _lineRenderer.enabled = true;

            // Cancel previous if exists
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();

            // Start lifetime management using UniTask (no Update needed)
            DeactivateAfterDuration(_cancellationTokenSource.Token).Forget();

            // Check collisions immediately (one-time hit)
            CheckCollisionsWithEnemies();
        }

        private async UniTaskVoid DeactivateAfterDuration(CancellationToken cancellationToken)
        {
            await UniTask.Delay((int)(_laserSettings.Duration * 1000), cancellationToken: cancellationToken);

            if (!cancellationToken.IsCancellationRequested)
            {
                _signalBus?.Fire(new LaserDeactivatedSignal());
            }
        }

        private void OnLaserDeactivated(LaserDeactivatedSignal signal)
        {
            Deactivate();
        }

        private void UpdateLaserVisualization()
        {
            if (_lineRenderer == null || !_isActive)
            {
                return;
            }

            // Calculate end position using settings
            Vector2 endPosition = _startPosition + _direction * _laserSettings.Range;

            // Set LineRenderer positions
            _lineRenderer.SetPosition(0, new Vector3(_startPosition.x, _startPosition.y, 0f));
            _lineRenderer.SetPosition(1, new Vector3(endPosition.x, endPosition.y, 0f));
        }

        private void CheckCollisionsWithEnemies()
        {
            if (_enemySpawner == null || _enemySpawner.ActiveEnemies == null)
            {
                return;
            }

            // Calculate laser rectangle bounds using settings
            Vector2 endPosition = _startPosition + _direction * _laserSettings.Range;
            Vector2 laserCenter = (_startPosition + endPosition) * 0.5f;
            float laserLength = _laserSettings.Range;
            float laserWidth = _laserSettings.Width;
            float laserAngle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;

            // Check collision with each active enemy
            for (int i = _enemySpawner.ActiveEnemies.Count - 1; i >= 0; i--)
            {
                var enemy = _enemySpawner.ActiveEnemies[i];
                if (enemy == null || enemy.Entity == null)
                {
                    continue;
                }

                var transformComponent = enemy.Entity.GetComponent<TransformComponent>();
                if (transformComponent == null)
                {
                    continue;
                }

                Vector2 enemyPosition = transformComponent.Position;
                float enemyRadius = GetEnemyRadius(enemy);

                // Check if enemy is inside laser rectangle
                if (IsPointInRotatedRectangle(enemyPosition, laserCenter, laserLength, laserWidth, laserAngle, enemyRadius))
                {
                    // Destroy enemy
                    HandleEnemyHit(enemy);
                }
            }
        }

        private float GetEnemyRadius(EnemyView enemy)
        {
            // Get enemy radius from collider or use default
            var collider = enemy.GetComponent<CircleCollider2D>();
            if (collider != null)
            {
                return collider.radius;
            }

            // Default radius based on enemy type
            return 0.5f;
        }

        /// <summary>
        /// Check if a point (with radius) is inside a rotated rectangle
        /// </summary>
        private bool IsPointInRotatedRectangle(Vector2 point, Vector2 rectCenter, float rectLength, float rectWidth, float rectAngle, float pointRadius)
        {
            // Transform point to rectangle's local space (rotate around rectangle center)
            Vector2 localPoint = point - rectCenter;
            float angleRad = -rectAngle * Mathf.Deg2Rad;
            float cos = Mathf.Cos(angleRad);
            float sin = Mathf.Sin(angleRad);

            // Rotate point to rectangle's local coordinate system
            Vector2 rotatedPoint = new Vector2(
                localPoint.x * cos - localPoint.y * sin,
                localPoint.x * sin + localPoint.y * cos
            );

            // Check if point (with radius) is inside rectangle bounds
            // Rectangle extends from -length/2 to +length/2 along X axis
            // and from -width/2 to +width/2 along Y axis
            float halfLength = rectLength * 0.5f;
            float halfWidth = rectWidth * 0.5f;

            // Expand rectangle bounds by point radius
            bool insideX = rotatedPoint.x >= -halfLength - pointRadius && rotatedPoint.x <= halfLength + pointRadius;
            bool insideY = rotatedPoint.y >= -halfWidth - pointRadius && rotatedPoint.y <= halfWidth + pointRadius;

            return insideX && insideY;
        }

        private void HandleEnemyHit(EnemyView enemy)
        {
            if (enemy == null || enemy.Entity == null)
            {
                return;
            }

            // Destroy enemy instantly without fragmentation (laser destroys everything completely)
            enemy.HandleInstaDeath();
        }

        private void Deactivate()
        {
            _isActive = false;

            if (_lineRenderer != null)
            {
                _lineRenderer.enabled = false;
            }
        }
    }
}


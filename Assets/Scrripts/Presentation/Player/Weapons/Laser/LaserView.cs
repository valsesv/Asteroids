using System;
using UnityEngine;
using Zenject;
using Asteroids.Core.Entity.Components;
using Asteroids.Presentation.Enemies;
using Cysharp.Threading.Tasks;
using System.Threading;
using Asteroids.Core.Weapons;

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
        private LaserSettings _laserSettings;
        private CancellationTokenSource _cancellationTokenSource;

        private Vector2 _startPosition;
        private Vector2 _direction;
        private bool _isActive;

        [Inject]
        public void Construct(SignalBus signalBus, LaserSettings laserSettings)
        {
            _signalBus = signalBus;
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
            // Calculate laser box bounds using settings
            Vector2 endPosition = _startPosition + _direction * _laserSettings.Range;
            Vector2 laserCenter = (_startPosition + endPosition) * 0.5f;
            Vector2 laserSize = new Vector2(_laserSettings.Range, _laserSettings.Width);
            float laserAngle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;

            // Use OverlapBoxAll to find all colliders in the laser area
            Collider2D[] colliders = Physics2D.OverlapBoxAll(laserCenter, laserSize, laserAngle);

            // Process all colliders and check for enemies
            foreach (var collider in colliders)
            {
                if (collider == null)
                {
                    continue;
                }

                var enemy = collider.GetComponent<EnemyView>();
                if (enemy == null)
                {
                    enemy = collider.GetComponentInParent<EnemyView>();
                }

                if (enemy != null && enemy.Entity != null)
                {
                    enemy.HandleInstaDeath();
                }
            }
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


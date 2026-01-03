using System;
using UnityEngine;
using Zenject;
using Asteroids.Presentation.Enemies;
using Asteroids.Core.Player;
using Cysharp.Threading.Tasks;
using System.Threading;
using Asteroids.Core.Weapons;
using UnityEngine.Assertions;

namespace Asteroids.Presentation.Player
{
    public class LaserPresentation : MonoBehaviour, IInitializable, IDisposable
    {
        [SerializeField] private LineRenderer _lineRenderer;

        private LaserSettings _laserSettings;
        private LaserShootingLogic _laserShootingLogic;
        private CancellationTokenSource _cancellationTokenSource;

        private Vector2 _startPosition;
        private Vector2 _direction;
        private bool _isActive;

        [Inject]
        public void Construct(LaserSettings laserSettings, LaserShootingLogic laserShootingLogic)
        {
            _laserSettings = laserSettings;
            _laserShootingLogic = laserShootingLogic;
        }

        public void Initialize()
        {
            Assert.IsNotNull(_lineRenderer, "LineRenderer is not assigned in LaserPresentation!");

            _laserShootingLogic.OnLaserShot += OnLaserShot;

            _lineRenderer.startWidth = _laserSettings.Width;
            _lineRenderer.endWidth = _laserSettings.Width;
            _lineRenderer.positionCount = 2;
            _lineRenderer.useWorldSpace = true;
            _lineRenderer.enabled = false;
        }

        public void Dispose()
        {
            if (_laserShootingLogic != null)
            {
                _laserShootingLogic.OnLaserShot -= OnLaserShot;
            }
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }

        private void OnLaserShot(Vector2 startPosition, Vector2 direction)
        {
            _startPosition = startPosition;
            _direction = direction;
            _isActive = true;

            UpdateLaserVisualization();
            _lineRenderer.enabled = true;

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();

            DeactivateAfterDuration(_cancellationTokenSource.Token).Forget();

            CheckCollisionsWithEnemies();
        }

        private async UniTask DeactivateAfterDuration(CancellationToken cancellationToken)
        {
            await UniTask.Delay((int)(_laserSettings.Duration * 1000), cancellationToken: cancellationToken);

            if (!cancellationToken.IsCancellationRequested)
            {
                Deactivate();
            }
        }

        private void UpdateLaserVisualization()
        {
            if (!_isActive)
            {
                return;
            }

            Vector2 endPosition = _startPosition + _direction * _laserSettings.Range;

            _lineRenderer.SetPosition(0, new Vector3(_startPosition.x, _startPosition.y, 0f));
            _lineRenderer.SetPosition(1, new Vector3(endPosition.x, endPosition.y, 0f));
        }

        private void CheckCollisionsWithEnemies()
        {
            Vector2 endPosition = _startPosition + _direction * _laserSettings.Range;
            Vector2 laserCenter = (_startPosition + endPosition) * 0.5f;
            Vector2 laserSize = new Vector2(_laserSettings.Range, _laserSettings.Width);
            float laserAngle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;

            Collider2D[] colliders = Physics2D.OverlapBoxAll(laserCenter, laserSize, laserAngle);

            foreach (var collider in colliders)
            {
                if (collider == null)
                {
                    continue;
                }

                var enemy = collider.GetComponent<EnemyPresentation>();
                if (enemy == null)
                {
                    enemy = collider.GetComponentInParent<EnemyPresentation>();
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
            _lineRenderer.enabled = false;
        }
    }
}


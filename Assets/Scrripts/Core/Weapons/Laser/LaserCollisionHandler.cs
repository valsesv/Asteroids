using UnityEngine;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;
using Asteroids.Core.Enemies;
using Zenject;
using Cysharp.Threading.Tasks;

namespace Asteroids.Core.Player
{
    /// <summary>
    /// Component that handles laser collisions with all enemies in its path
    /// </summary>
    public class LaserCollisionHandler : IComponent
    {
        private readonly GameEntity _laserEntity;
        private readonly LaserComponent _laserComponent;
        private readonly TransformComponent _transform;
        private readonly SignalBus _signalBus;
        private readonly LaserSettings _laserSettings;

        public LaserCollisionHandler(
            GameEntity laserEntity,
            LaserComponent laserComponent,
            TransformComponent transform,
            SignalBus signalBus,
            LaserSettings laserSettings)
        {
            _laserEntity = laserEntity;
            _laserComponent = laserComponent;
            _transform = transform;
            _signalBus = signalBus;
            _laserSettings = laserSettings;
        }

        /// <summary>
        /// Check collisions with all enemies and destroy them
        /// </summary>
        public void CheckCollisions(System.Collections.Generic.List<GameEntity> enemies)
        {
            if (enemies == null || enemies.Count == 0)
            {
                return;
            }

            Vector2 laserPosition = _transform.Position;
            float angle = _transform.Rotation * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            // Calculate laser beam endpoints (very long range)
            float range = 2000f; // Large range to cover entire screen
            Vector2 endPoint = laserPosition + direction * range;

            // Check each enemy for intersection with laser beam
            foreach (var enemy in enemies)
            {
                if (enemy == null)
                {
                    continue;
                }

                var enemyTransform = enemy.GetComponent<TransformComponent>();
                if (enemyTransform == null)
                {
                    continue;
                }

                // Simple point-to-line distance check
                // If enemy is close enough to laser line, it's hit
                float distanceToLine = PointToLineDistance(enemyTransform.Position, laserPosition, endPoint);
                
                if (distanceToLine <= _laserSettings.Width * 0.5f + 0.5f) // Add small margin for enemy size
                {
                    // Check if enemy is in front of laser (not behind)
                    Vector2 toEnemy = enemyTransform.Position - laserPosition;
                    if (Vector2.Dot(toEnemy, direction) > 0)
                    {
                        HandleEnemyHit(enemy);
                    }
                }
            }
        }

        private float PointToLineDistance(Vector2 point, Vector2 lineStart, Vector2 lineEnd)
        {
            Vector2 line = lineEnd - lineStart;
            float lineLength = line.magnitude;
            
            if (lineLength < 0.001f)
            {
                return Vector2.Distance(point, lineStart);
            }

            Vector2 lineNormalized = line / lineLength;
            Vector2 toPoint = point - lineStart;
            float projection = Vector2.Dot(toPoint, lineNormalized);

            // Clamp projection to line segment
            projection = Mathf.Clamp(projection, 0f, lineLength);

            Vector2 closestPoint = lineStart + lineNormalized * projection;
            return Vector2.Distance(point, closestPoint);
        }

        private void HandleEnemyHit(GameEntity enemyEntity)
        {
            var enemyComponent = enemyEntity.GetComponent<EnemyComponent>();
            if (enemyComponent == null)
            {
                return;
            }

            // Destroy enemy (laser destroys everything)
            _signalBus?.Fire(new EnemyDestroyedSignal { Entity = enemyEntity });
        }
    }
}


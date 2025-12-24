using UnityEngine;
using Zenject;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;

namespace Asteroids.Core.Player
{
    /// <summary>
    /// Factory for creating laser entities
    /// </summary>
    public class LaserFactory
    {
        private readonly SignalBus _signalBus;
        private readonly ScreenBounds _screenBounds;
        private readonly LaserSettings _laserSettings;

        public LaserFactory(SignalBus signalBus, ScreenBounds screenBounds, LaserSettings laserSettings)
        {
            _signalBus = signalBus;
            _screenBounds = screenBounds;
            _laserSettings = laserSettings;
        }

        /// <summary>
        /// Create a laser entity
        /// </summary>
        public GameEntity CreateLaser(Vector2 position, Vector2 direction, float duration, float width)
        {
            // Calculate rotation from direction
            float rotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            var entity = new GameEntity(position, rotation, _signalBus);

            // Add laser component
            var laserComponent = new LaserComponent(duration, width);
            entity.AddComponent(laserComponent);

            // Add transform component (laser doesn't move, it's a beam)
            var transform = entity.GetComponent<TransformComponent>();

            // Add collision handler
            var collisionHandler = new LaserCollisionHandler(entity, laserComponent, transform, _signalBus, _laserSettings);
            entity.AddComponent(collisionHandler);

            // Fire signal that laser was created
            _signalBus?.Fire(new LaserCreatedSignal 
            { 
                Entity = entity,
                Position = position,
                Direction = direction,
                Duration = duration,
                Width = width
            });

            return entity;
        }
    }
}


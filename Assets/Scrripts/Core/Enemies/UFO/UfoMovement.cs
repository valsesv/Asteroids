using UnityEngine;
using Zenject;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;

namespace Asteroids.Core.Enemies
{
    /// <summary>
    /// Component that handles UFO movement - chases the player
    /// </summary>
    public class UfoMovement : ITickableComponent
    {
        private readonly GameEntity _entity;
        private readonly PhysicsComponent _physics;
        private readonly TransformComponent _transform;
        private readonly TransformComponent _playerTransform;
        private readonly float _speed;

        public UfoMovement(
            GameEntity entity,
            PhysicsComponent physics,
            TransformComponent transform,
            TransformComponent playerTransform,
            SignalBus signalBus,
            float speed)
        {
            _entity = entity;
            _physics = physics;
            _transform = transform;
            _playerTransform = playerTransform;
            _speed = speed;
        }

        public void Tick()
        {
            if (_playerTransform == null)
            {
                return;
            }

            Vector2 playerPosition = _playerTransform.Position;
            Vector2 ufoPosition = _transform.Position;

            // Calculate direction to player
            Vector2 directionToPlayer = (playerPosition - ufoPosition).normalized;

            // Set velocity directly towards player (no acceleration, direct motion)
            _physics.SetVelocity(directionToPlayer * _speed);

            // Clamp speed to ensure it doesn't exceed maximum
            _physics.ClampSpeed(_speed);
        }
    }
}


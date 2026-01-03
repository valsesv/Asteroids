using UnityEngine;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;

namespace Asteroids.Core.Enemies
{
    public class UfoMovement : ITickableComponent
    {
        private readonly PhysicsComponent _physics;
        private readonly TransformComponent _transform;
        private readonly TransformComponent _playerTransform;
        private readonly float _speed;

        public UfoMovement(
            GameEntity entity,
            TransformComponent playerTransform,
            float speed)
        {
            _physics = entity.GetComponent<PhysicsComponent>();
            _transform = entity.GetComponent<TransformComponent>();
            _playerTransform = playerTransform;
            _speed = speed;
        }

        public void Tick()
        {
            Vector2 playerPosition = _playerTransform.Position;
            Vector2 ufoPosition = _transform.Position;
            Vector2 motionDirection = (playerPosition - ufoPosition).normalized;

            _physics.SetVelocity(motionDirection * _speed);
            _physics.ClampSpeed(_speed);
        }
    }
}


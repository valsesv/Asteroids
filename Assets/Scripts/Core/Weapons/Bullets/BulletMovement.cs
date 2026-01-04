using UnityEngine;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;

namespace Asteroids.Core.Player
{
    public class BulletMovement : IComponent
    {
        private readonly PhysicsComponent _physics;
        private Vector2 _direction;
        private readonly float _speed;

        public BulletMovement(
            PhysicsComponent physics,
            float speed)
        {
            _physics = physics;
            _speed = speed;
        }

        public void SetDirection(Vector2 direction)
        {
            _direction = direction.normalized;
            _physics.SetVelocity(_direction * _speed);
            _physics.ClampSpeed(_speed);
        }
    }
}
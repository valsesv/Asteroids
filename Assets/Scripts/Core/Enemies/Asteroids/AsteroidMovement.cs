using UnityEngine;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;

namespace Asteroids.Core.Enemies
{
    public class AsteroidMovement : IComponent
    {
        private readonly PhysicsComponent _physics;
        private readonly float _speed;

        public AsteroidMovement(GameEntity entity, float speed)
        {
            _physics = entity.GetComponent<PhysicsComponent>();
            _speed = speed;
        }

        public void SetDirection(Vector2 direction)
        {
            _physics.SetVelocity(direction * _speed);
            _physics.ClampSpeed(_speed);
        }
    }
}
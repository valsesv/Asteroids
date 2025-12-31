using UnityEngine;
using Zenject;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;

namespace Asteroids.Core.Enemies
{
    /// <summary>
    /// Component that handles asteroid movement and collisions
    /// Sets initial velocity in given direction with constant speed (no friction)
    /// </summary>
    public class AsteroidMovement : IComponent
    {
        private readonly GameEntity _entity;
        private readonly PhysicsComponent _physics;
        private Vector2 _direction;
        private readonly float _speed;

        public AsteroidMovement(
            GameEntity entity,
            float speed,
            SignalBus signalBus)
        {
            _physics = entity.GetComponent<PhysicsComponent>();
            _direction = Vector2.zero;
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


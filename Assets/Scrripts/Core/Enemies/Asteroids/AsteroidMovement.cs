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
        private readonly SignalBus _signalBus;
        private readonly Vector2 _direction;
        private readonly float _speed;

        public AsteroidMovement(
            GameEntity entity,
            PhysicsComponent physics,
            Vector2 direction,
            float speed,
            SignalBus signalBus)
        {
            _entity = entity;
            _physics = physics;
            _direction = direction.normalized;
            _speed = speed;
            _signalBus = signalBus;

            // Set initial velocity - asteroid moves with constant speed in given direction (no friction)
            _physics.SetVelocity(_direction * _speed, _signalBus);
        }

        // This component will be used for collision handling in the future
        // For now, velocity is set once in constructor and maintained by PhysicsComponent
    }
}


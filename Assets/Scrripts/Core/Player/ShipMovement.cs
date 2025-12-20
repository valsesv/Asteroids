using UnityEngine;
using Zenject;
using Asteroids.Core.PlayerInput;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;

namespace Asteroids.Core.Player
{
    /// <summary>
    /// Component that handles ship movement with acceleration and inertia
    /// </summary>
    public class ShipMovement : ITickableComponent
    {
        private readonly ShipModel _entity;
        private readonly MovementSettings _movementSettings;
        private readonly SignalBus _signalBus;
        private readonly IInputProvider _inputProvider;
        private readonly PhysicsComponent _physics;

        /// <summary>
        /// Create movement component with all required dependencies
        /// </summary>
        public ShipMovement(ShipModel entity, MovementSettings settings, IInputProvider inputProvider, PhysicsComponent physics, SignalBus signalBus)
        {
            _entity = entity;
            _movementSettings = settings;
            _inputProvider = inputProvider;
            _physics = physics;
            _signalBus = signalBus;
        }

        public void Tick()
        {
            // Check if entity has CanControl property (ShipModel specific)
            if (_entity.CanControl)
            {
                HandleMovement();
            }
        }

        private void HandleMovement()
        {
            Vector2 input = _inputProvider.GetMovementInput();

            if (input.magnitude < 0.01f)
            {
                return;
            }

            // Apply acceleration (components will fire their own signals)
            Vector2 acceleration = input * _movementSettings.Acceleration;
            _physics.AddVelocity(acceleration * Time.deltaTime, _signalBus);

            // Limit max speed
            _physics.ClampSpeed(_movementSettings.MaxSpeed);
        }
    }
}


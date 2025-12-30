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
        private readonly GameEntity _entity;
        private readonly ShipComponent _shipComponent;
        private readonly MovementSettings _movementSettings;
        private readonly SignalBus _signalBus;
        private readonly IInputProvider _inputProvider;
        private readonly PhysicsComponent _physics;
        private readonly TransformComponent _transform;

        /// <summary>
        /// Create movement component with all required dependencies
        /// </summary>
        public ShipMovement(GameEntity entity, MovementSettings settings, IInputProvider inputProvider, PhysicsComponent physics, SignalBus signalBus)
        {
            _entity = entity;
            _shipComponent = entity.GetComponent<ShipComponent>();
            _movementSettings = settings;
            _inputProvider = inputProvider;
            _physics = physics;
            _signalBus = signalBus;
            _transform = entity.GetComponent<TransformComponent>();
        }

        public void Tick()
        {
            // Check if entity can be controlled via ShipComponent
            if (_shipComponent != null && _shipComponent.CanControl)
            {
                HandleRotation();
                HandleMovement();
            }
        }

        private void HandleRotation()
        {
            float rotationInput = _inputProvider.GetRotationInput();

            if (Mathf.Abs(rotationInput) < 0.01f)
            {
                return;
            }

            // Apply rotation (inverted: negative input rotates right, positive rotates left)
            float rotationDelta = -rotationInput * _movementSettings.RotationSpeed * Time.deltaTime;
            float newRotation = _transform.Rotation + rotationDelta;
            _transform.SetRotation(newRotation);
        }

        private void HandleMovement()
        {
            float forwardInput = _inputProvider.GetForwardInput();

            if (Mathf.Abs(forwardInput) < 0.01f)
            {
                return;
            }

            float angle = (_transform.Rotation + 90f) * Mathf.Deg2Rad;
            Vector2 forwardDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            // Apply acceleration in forward/backward direction
            Vector2 acceleration = forwardDirection * forwardInput * _movementSettings.Acceleration;
            _physics.AddVelocity(acceleration * Time.deltaTime);

            // Limit max speed
            _physics.ClampSpeed(_movementSettings.MaxSpeed);
        }
    }
}


using UnityEngine;
using Zenject;
using Asteroids.Core.Signals;
using Asteroids.Core.PlayerInput;
using Asteroids.Core.Entity.Components;

namespace Asteroids.Core.Player
{
    /// <summary>
    /// Handles ship movement with acceleration and inertia
    /// Works with Entity component system
    /// </summary>
    public class ShipMovement : ITickable
    {
        private readonly ShipModel _shipModel;
        private readonly MovementSettings _movementSettings;
        private readonly SignalBus _signalBus;
        private readonly IInputProvider _inputProvider;

        private readonly ShipPositionChangedSignal _positionSignal = new ShipPositionChangedSignal();
        private readonly ShipVelocityChangedSignal _velocitySignal = new ShipVelocityChangedSignal();

        public ShipMovement(ShipModel shipModel, MovementSettings settings, SignalBus signalBus, IInputProvider inputProvider)
        {
            _shipModel = shipModel;
            _movementSettings = settings;
            _signalBus = signalBus;
            _inputProvider = inputProvider;
        }

        public void Tick()
        {
            var transform = _shipModel.GetTransform();
            var physics = _shipModel.GetPhysics();

            if (transform == null || physics == null)
            {
                return;
            }

            if (_shipModel.CanControl)
            {
                HandleMovement(physics);
            }

            ApplyFriction(physics);
            UpdatePosition(transform, physics);
        }

        private void HandleMovement(PhysicsComponent physics)
        {
            Vector2 input = _inputProvider.GetMovementInput();

            if (input.magnitude < 0.01f)
            {
                return;
            }

            // Apply acceleration
            Vector2 acceleration = input * _movementSettings.Acceleration;
            physics.AddVelocity(acceleration * Time.deltaTime);

            // Limit max speed
            physics.ClampSpeed(_movementSettings.MaxSpeed);
        }

        private void ApplyFriction(PhysicsComponent physics)
        {
            physics.ApplyFriction(_movementSettings.Friction, Time.deltaTime);
        }

        private void UpdatePosition(TransformComponent transform, PhysicsComponent physics)
        {
            // Update position based on velocity
            transform.Move(physics.Velocity * Time.deltaTime);

            // Fire signals
            _positionSignal.X = transform.Position.x;
            _positionSignal.Y = transform.Position.y;
            _positionSignal.Rotation = transform.Rotation;
            _signalBus.Fire(_positionSignal);

            _velocitySignal.VelocityX = physics.Velocity.x;
            _velocitySignal.VelocityY = physics.Velocity.y;
            _velocitySignal.Speed = physics.Velocity.magnitude;
            _signalBus.Fire(_velocitySignal);
        }
    }
}


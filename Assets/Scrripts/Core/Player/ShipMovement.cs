using UnityEngine;
using Zenject;
using Asteroids.Core.Signals;
using Asteroids.Core.PlayerInput;

namespace Asteroids.Core.Player
{
    /// <summary>
    /// Handles ship movement with acceleration and inertia
    /// </summary>
    public partial class ShipMovement : ITickable
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
            if (_shipModel.CanControl)
            {
                HandleMovement();
            }

            ApplyFriction();
            UpdatePosition();
        }

        private void HandleMovement()
        {
            Vector2 input = _inputProvider.GetMovementInput();

            if (input.magnitude < 0.01f)
            {
                return;
            }
            _shipModel.Velocity += input * _movementSettings.Acceleration * Time.deltaTime;

            // Limit max speed
            if (_shipModel.Velocity.magnitude > _movementSettings.MaxSpeed)
            {
                _shipModel.Velocity = _shipModel.Velocity.normalized * _movementSettings.MaxSpeed;
            }
        }

        private void ApplyFriction()
        {
            _shipModel.Velocity *= Mathf.Pow(_movementSettings.Friction, Time.deltaTime);
        }

        private void UpdatePosition()
        {
            _shipModel.Position += _shipModel.Velocity * Time.deltaTime;

            _positionSignal.X = _shipModel.Position.x;
            _positionSignal.Y = _shipModel.Position.y;
            _positionSignal.Rotation = _shipModel.Rotation;
            _signalBus.Fire(_positionSignal);

            _velocitySignal.VelocityX = _shipModel.Velocity.x;
            _velocitySignal.VelocityY = _shipModel.Velocity.y;
            _velocitySignal.Speed = _shipModel.Velocity.magnitude;
            _signalBus.Fire(_velocitySignal);
        }
    }
}


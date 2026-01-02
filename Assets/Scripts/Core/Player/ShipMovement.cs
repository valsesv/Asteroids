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
                // Check if we have direction input (joystick mode) or separate rotation/forward input (keyboard mode)
                Vector2 directionInput = _inputProvider.GetDirectionInput();

                if (directionInput.magnitude > 0.01f)
                {
                    // Joystick mode: move and rotate towards joystick direction
                    HandleDirectionBasedMovement(directionInput);
                }
                else
                {
                    // Keyboard mode: separate rotation and forward movement
                    HandleRotation();
                    HandleMovement();
                }
            }
        }

        private void HandleDirectionBasedMovement(Vector2 directionInput)
        {
            // Calculate target angle from joystick direction
            // Joystick direction: Y is forward/backward, X is left/right
            // In Unity 2D, 0° = right, 90° = up, -90° = down, 180° = left
            // But our ship uses: 0° = up, 90° = right, -90° = left, 180° = down
            // Invert X to fix rotation direction
            float targetAngle = Mathf.Atan2(-directionInput.x, directionInput.y) * Mathf.Rad2Deg;

            // Current rotation (ship's forward direction)
            float currentAngle = _transform.Rotation;

            // Calculate angle difference (normalize to -180..180)
            float angleDiff = Mathf.DeltaAngle(currentAngle, targetAngle);

            // Rotate towards target angle smoothly
            float rotationSpeed = _movementSettings.RotationSpeed;
            float maxRotationDelta = rotationSpeed * Time.deltaTime;
            float rotationDelta = Mathf.Clamp(angleDiff, -maxRotationDelta, maxRotationDelta);
            float newRotation = currentAngle + rotationDelta;
            _transform.SetRotation(newRotation);

            // Apply acceleration in the direction of joystick
            // Normalize direction so speed doesn't depend on joystick distance
            Vector2 worldDirection = directionInput.normalized;

            // Use constant acceleration (not dependent on joystick magnitude)
            Vector2 acceleration = worldDirection * _movementSettings.Acceleration;
            _physics.AddVelocity(acceleration * Time.deltaTime);

            // Limit max speed
            _physics.ClampSpeed(_movementSettings.MaxSpeed);
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


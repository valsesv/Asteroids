using UnityEngine;
using Zenject;
using Asteroids.Core.PlayerInput;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;

namespace Asteroids.Core.Player
{
    public class ShipMovement : ITickableComponent
    {
        private readonly ShipComponent _shipComponent;
        private readonly MovementSettings _movementSettings;
        private readonly IInputProvider _inputProvider;
        private readonly PhysicsComponent _physics;
        private readonly TransformComponent _transform;

        public ShipMovement(GameEntity entity, MovementSettings settings, IInputProvider inputProvider, PhysicsComponent physics, SignalBus signalBus)
        {
            _shipComponent = entity.GetComponent<ShipComponent>();
            _movementSettings = settings;
            _inputProvider = inputProvider;
            _physics = physics;
            _transform = entity.GetComponent<TransformComponent>();
        }

        public void Tick()
        {
            if (_shipComponent != null && _shipComponent.CanControl)
            {
                Vector2 directionInput = _inputProvider.GetDirectionInput();

                if (directionInput.magnitude > 0.01f)
                {
                    HandleDirectionBasedMovement(directionInput);
                }
                else
                {
                    HandleRotation();
                    HandleMovement();
                }
            }
        }

        private void HandleDirectionBasedMovement(Vector2 directionInput)
        {
            float targetAngle = Mathf.Atan2(-directionInput.x, directionInput.y) * Mathf.Rad2Deg;
            float currentAngle = _transform.Rotation;
            float angleDiff = Mathf.DeltaAngle(currentAngle, targetAngle);
            float rotationSpeed = _movementSettings.RotationSpeed;
            float maxRotationDelta = rotationSpeed * Time.deltaTime;
            float rotationDelta = Mathf.Clamp(angleDiff, -maxRotationDelta, maxRotationDelta);
            float newRotation = currentAngle + rotationDelta;
            _transform.SetRotation(newRotation);

            Vector2 worldDirection = directionInput.normalized;
            Vector2 acceleration = worldDirection * _movementSettings.Acceleration;
            _physics.AddVelocity(acceleration * Time.deltaTime);
            _physics.ClampSpeed(_movementSettings.MaxSpeed);
        }

        private void HandleRotation()
        {
            float rotationInput = _inputProvider.GetRotationInput();

            if (Mathf.Abs(rotationInput) < 0.01f)
            {
                return;
            }

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
            Vector2 acceleration = forwardDirection * forwardInput * _movementSettings.Acceleration;
            _physics.AddVelocity(acceleration * Time.deltaTime);
            _physics.ClampSpeed(_movementSettings.MaxSpeed);
        }
    }
}


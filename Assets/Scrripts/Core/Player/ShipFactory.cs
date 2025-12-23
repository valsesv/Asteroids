using UnityEngine;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;
using Asteroids.Core.PlayerInput;
using Zenject;

namespace Asteroids.Core.Player
{
    /// <summary>
    /// Factory for creating ship entities
    /// Creates GameEntity and adds ship-specific components
    /// </summary>
    public static class ShipFactory
    {
        /// <summary>
        /// Create a ship entity with all required components
        /// </summary>
        public static GameEntity CreateShip(
            StartPositionSettings startPositionSettings,
            MovementSettings movementSettings,
            HealthSettings healthSettings,
            SignalBus signalBus,
            IInputProvider inputProvider,
            ScreenBounds screenBounds)
        {
            var entity = new GameEntity(startPositionSettings.Position, startPositionSettings.Rotation, signalBus);

            // Add ship component
            var shipComponent = new ShipComponent();
            entity.AddComponent(shipComponent);

            // Add physics component
            var transform = entity.GetComponent<TransformComponent>();
            var physicsComponent = new PhysicsComponent(transform, signalBus, mass: 1f, frictionCoefficient: movementSettings.Friction);
            entity.AddComponent(physicsComponent);

            // Add ship movement component
            var shipMovement = new ShipMovement(entity, movementSettings, inputProvider, physicsComponent, signalBus);
            entity.AddComponent(shipMovement);

            // Add health component
            var healthComponent = new HealthComponent(signalBus, healthSettings.MaxHealth);
            entity.AddComponent(healthComponent);

            // Add damage handler
            var damageHandler = new DamageHandler(healthComponent, entity, signalBus, healthSettings.InvincibilityDuration, healthSettings.BounceForce);
            entity.AddComponent(damageHandler);

            // Add screen wrap component
            var screenWrap = new ScreenWrapComponent(transform, screenBounds, signalBus);
            entity.AddComponent(screenWrap);

            return entity;
        }
    }
}


using UnityEngine;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;
using Asteroids.Core.PlayerInput;
using Zenject;

namespace Asteroids.Core.Player
{
    /// <summary>
    /// Ship model - a GameEntity with ship-specific behavior
    /// Extends GameEntity to use component system directly
    /// </summary>
    public class ShipModel : GameEntity
    {
        private PhysicsComponent _physicsComponent;
        private ShipMovement _shipMovement;

        public ShipModel(StartPositionSettings startPositionSettings, MovementSettings movementSettings, SignalBus signalBus, IInputProvider inputProvider, ScreenBounds screenBounds)
            : base(startPositionSettings.Position, startPositionSettings.Rotation)
        {
            CanControl = true;

            var transform = GetComponent<TransformComponent>();
            _physicsComponent = new PhysicsComponent(transform, signalBus, mass: 1f, frictionCoefficient: movementSettings.Friction);
            AddComponent(_physicsComponent);

            _shipMovement = new ShipMovement(this, movementSettings, inputProvider, _physicsComponent, signalBus);
            AddComponent(_shipMovement);

            // Add screen wrap component for teleportation at screen boundaries
            var screenWrap = new ScreenWrapComponent(transform, screenBounds, signalBus);
            AddComponent(screenWrap);
        }

        public bool CanControl { get; private set; }
    }
}


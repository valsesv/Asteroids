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
        private HealthComponent _healthComponent;
        private DamageHandler _damageHandler;

        public ShipModel(StartPositionSettings startPositionSettings, MovementSettings movementSettings, HealthSettings healthSettings, SignalBus signalBus, IInputProvider inputProvider, ScreenBounds screenBounds)
            : base(startPositionSettings.Position, startPositionSettings.Rotation, signalBus)
        {
            CanControl = true;

            var transform = GetComponent<TransformComponent>();
            _physicsComponent = new PhysicsComponent(transform, signalBus, mass: 1f, frictionCoefficient: movementSettings.Friction);
            AddComponent(_physicsComponent);

            _shipMovement = new ShipMovement(this, movementSettings, inputProvider, _physicsComponent, signalBus);
            AddComponent(_shipMovement);

            _healthComponent = new HealthComponent(signalBus, healthSettings.MaxHealth);
            AddComponent(_healthComponent);

            _damageHandler = new DamageHandler(_healthComponent, this, signalBus, healthSettings.InvincibilityDuration);
            AddComponent(_damageHandler);

            var screenWrap = new ScreenWrapComponent(transform, screenBounds, signalBus);
            AddComponent(screenWrap);
        }

        public bool CanControl { get; set; }
    }
}


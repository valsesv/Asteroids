using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;
using Asteroids.Core.PlayerInput;
using Zenject;
using Asteroids.Core.Weapons;

namespace Asteroids.Core.Player
{
    public static class ShipFactory
    {
        public static GameEntity CreateShip(
            StartPositionSettings startPositionSettings,
            MovementSettings movementSettings,
            HealthSettings healthSettings,
            WeaponSettings weaponSettings,
            SignalBus signalBus,
            IInputProvider inputProvider,
            ScreenBounds screenBounds)
        {
            var entity = new GameEntity(startPositionSettings.Position, startPositionSettings.Rotation, signalBus);

            var shipComponent = new ShipComponent(entity);
            entity.AddComponent(shipComponent);

            var transform = entity.GetComponent<TransformComponent>();
            var physicsComponent = new PhysicsComponent(transform, signalBus, frictionCoefficient: movementSettings.Friction);
            entity.AddComponent(physicsComponent);

            var shipMovement = new ShipMovement(entity, movementSettings, inputProvider, physicsComponent, signalBus);
            entity.AddComponent(shipMovement);

            var healthComponent = new HealthComponent(signalBus, healthSettings.MaxHealth);
            entity.AddComponent(healthComponent);

            var damageHandler = new DamageHandler(healthComponent, entity, signalBus, healthSettings.InvincibilityDuration, healthSettings.BounceForce);
            entity.AddComponent(damageHandler);

            var screenWrap = new ScreenWrapComponent(transform, screenBounds);
            entity.AddComponent(screenWrap);

            var laserComponent = new LaserComponent(weaponSettings.Laser, signalBus);
            entity.AddComponent(laserComponent);

            var weaponShooting = new WeaponShooting(entity, inputProvider, weaponSettings, signalBus);
            entity.AddComponent(weaponShooting);

            return entity;
        }
    }
}


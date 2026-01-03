using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;
using Asteroids.Core.PlayerInput;
using Asteroids.Core.Weapons;
using Zenject;

namespace Asteroids.Core.Player
{
    public class ShipEntityBuilder
    {
        private readonly DiContainer _container;

        public ShipEntityBuilder(DiContainer container)
        {
            _container = container;
        }

        public GameEntity CreateShip(
            StartPositionSettings startPositionSettings,
            MovementSettings movementSettings,
            HealthSettings healthSettings,
            WeaponSettings weaponSettings,
            IInputProvider inputProvider,
            ScreenBounds screenBounds)
        {
            var entity = new GameEntity(startPositionSettings.Position, startPositionSettings.Rotation);

            var shipComponent = new ShipComponent(entity);
            entity.AddComponent(shipComponent);

            var transform = entity.GetComponent<TransformComponent>();
            var physicsComponent = new PhysicsComponent(transform, frictionCoefficient: movementSettings.Friction);
            entity.AddComponent(physicsComponent);

            var shipMovement = new ShipMovement(entity, movementSettings, inputProvider, physicsComponent);
            entity.AddComponent(shipMovement);

            var healthComponent = new HealthComponent(healthSettings.MaxHealth);
            entity.AddComponent(healthComponent);

            var damageHandler = new DamageHandler(healthComponent, entity, healthSettings.InvincibilityDuration, healthSettings.BounceForce);
            entity.AddComponent(damageHandler);

            var screenWrap = new ScreenWrapComponent(transform, screenBounds);
            entity.AddComponent(screenWrap);

            var laserComponent = new LaserComponent(weaponSettings.Laser);
            entity.AddComponent(laserComponent);

            var weaponShooting = _container.Instantiate<WeaponShooting>(new object[] { entity });
            entity.AddComponent(weaponShooting);

            weaponShooting.Initialize();

            RegisterTickableComponents(entity);

            return entity;
        }

        private void RegisterTickableComponents(GameEntity entity)
        {
            var tickableManager = _container.Resolve<TickableManager>();
            foreach (var tickableComponent in entity.GetTickableComponents())
            {
                tickableManager.Add(tickableComponent);
            }
        }
    }
}

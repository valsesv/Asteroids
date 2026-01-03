using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;
using Asteroids.Core.Weapons;
using Zenject;

namespace Asteroids.Core.Player
{
    public class BulletComponent : IInitializableComponent
    {
        public float Lifetime { get; set; }

        public BulletComponent()
        {
        }

        public void Initialize(GameEntity entity, DiContainer container)
        {
            var bulletSettings = container.Resolve<BulletSettings>();
            Lifetime = bulletSettings.Lifetime;

            var physicsComponent = entity.GetComponent<PhysicsComponent>();
            var movement = new BulletMovement(physicsComponent, bulletSettings.Speed);
            entity.AddComponent(movement);

            var bulletLifetime = new BulletLifetime(entity);
            entity.AddComponent(bulletLifetime);
        }
    }
}


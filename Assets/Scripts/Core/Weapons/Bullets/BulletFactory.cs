using UnityEngine;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;
using Zenject;

namespace Asteroids.Core.Player
{
    public static class BulletFactory
    {
        public static GameEntity CreateBullet(Vector2 position, float speed, float lifetime, SignalBus signalBus)
        {
            var entity = new GameEntity(position, 0f, signalBus);

            var bulletComponent = new BulletComponent(speed, lifetime);
            entity.AddComponent(bulletComponent);

            var transform = entity.GetComponent<TransformComponent>();
            var physicsComponent = new PhysicsComponent(transform, mass: 0.1f, frictionCoefficient: 1f);
            entity.AddComponent(physicsComponent);

            var movement = new BulletMovement(physicsComponent, speed);
            entity.AddComponent(movement);

            var bulletLifetime = new BulletLifetime(entity, signalBus);
            entity.AddComponent(bulletLifetime);

            return entity;
        }
    }
}


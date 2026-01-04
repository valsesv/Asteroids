using UnityEngine;
using Asteroids.Core.Entity.Components;
using Zenject;

namespace Asteroids.Core.Entity
{
    public class EntityFactory<T> where T : class, IComponent, new()
    {
        private readonly DiContainer _container;

        public EntityFactory(DiContainer container)
        {
            _container = container;
        }

        public GameEntity Create(Vector2 position, float rotation = 0f, float physicsMass = 1f)
        {
            var entity = new GameEntity(position, rotation);

            var transform = entity.GetComponent<TransformComponent>();
            var physicsComponent = new PhysicsComponent(transform, mass: physicsMass, frictionCoefficient: 1f);
            entity.AddComponent(physicsComponent);

            var component = new T();
            entity.AddComponent(component);

            if (component is IInitializableComponent initializable)
            {
                initializable.Initialize(entity, _container);
            }

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
using UnityEngine;
using Zenject;
using Asteroids.Core.Enemies;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;

namespace Asteroids.Presentation.Enemies
{
    public class AsteroidPresentation : EnemyPresentation
    {
        private AsteroidMovement _asteroidMovement;

        [Inject]
        public void Construct(
            EntityFactory<AsteroidComponent> entityFactory)
        {
            Vector2 position = new Vector2(transform.position.x, transform.position.y);
            float rotation = transform.eulerAngles.z;

            Entity = entityFactory.Create(position, rotation);
            _asteroidMovement = Entity.GetComponent<AsteroidMovement>();

            _container.BindInstance(Entity).AsSingle();
        }

        public void SetDirection(Vector2 direction)
        {
            _asteroidMovement.SetDirection(direction);
        }

        public override void GetDamage()
        {
            base.GetDamage();
            FragmentAsteroid();
        }

        private void FragmentAsteroid()
        {
            PhysicsComponent physicsComponent = Entity.GetComponent<PhysicsComponent>();
            Vector2 position = _transformComponent.Position;
            Vector2 velocity = physicsComponent.Velocity;
            _enemySpawner.FragmentAsteroid(this, position, velocity, Entity.GetComponent<AsteroidComponent>());
        }
    }
}


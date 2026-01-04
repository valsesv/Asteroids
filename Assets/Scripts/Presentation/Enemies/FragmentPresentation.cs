using UnityEngine;
using Zenject;
using Asteroids.Core.Enemies;
using Asteroids.Core.Entity;

namespace Asteroids.Presentation.Enemies
{
    public class FragmentPresentation : EnemyPresentation
    {
        private AsteroidMovement _asteroidMovement;

        [Inject]
        public void Construct(
            EntityFactory<FragmentComponent> entityFactory)
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
    }
}
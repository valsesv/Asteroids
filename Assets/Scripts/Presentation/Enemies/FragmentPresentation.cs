using UnityEngine;
using Zenject;
using Asteroids.Core.Enemies;
using Asteroids.Core.Entity;

namespace Asteroids.Presentation.Enemies
{
    public class FragmentPresentation : EnemyPresentation
    {
        [Inject]
        public void Construct(
            EntityFactory<FragmentComponent> entityFactory)
        {
            Vector2 position = new Vector2(transform.position.x, transform.position.y);
            float rotation = transform.eulerAngles.z;

            Entity = entityFactory.Create(position, rotation, physicsMass: 1f);

            _container.BindInstance(Entity).AsSingle();
        }

        public void SetDirection(Vector2 direction)
        {
            if (Entity == null)
            {
                return;
            }
            var movement = Entity.GetComponent<AsteroidMovement>();
            movement.SetDirection(direction);
        }

        public override void GetDamage()
        {
            base.GetDamage();

            _enemySpawner.ReturnEnemy(this);
        }
    }
}


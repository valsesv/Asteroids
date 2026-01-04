using UnityEngine;
using Zenject;
using Asteroids.Core.Enemies;
using Asteroids.Core.Entity;

namespace Asteroids.Presentation.Enemies
{
    public class UfoPresentation : EnemyPresentation
    {
        [Inject]
        public void Construct(
            EntityFactory<UfoComponent> entityFactory)
        {
            Vector2 position = new Vector2(transform.position.x, transform.position.y);
            float rotation = transform.eulerAngles.z;

            Entity = entityFactory.Create(position, rotation, physicsMass: 1f);

            _container.BindInstance(Entity).AsSingle();
        }
    }
}
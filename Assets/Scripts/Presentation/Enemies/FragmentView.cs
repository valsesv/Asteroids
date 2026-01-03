using UnityEngine;
using Zenject;
using Asteroids.Core.Enemies;
using Asteroids.Core.Entity;

namespace Asteroids.Presentation.Enemies
{
    public class FragmentView : EnemyView
    {
        [Inject]
        public void Construct(
            SignalBus signalBus,
            ScreenBounds screenBounds,
            EnemySettings enemySettings,
            EnemyFactory enemyFactory)
        {
            Vector2 position = new Vector2(transform.position.x, transform.position.y);
            float rotation = transform.eulerAngles.z;

            Entity = enemyFactory.CreateFragment(position, rotation, signalBus, enemySettings.FragmentSpeed, screenBounds);

            _container.BindInstance(Entity).AsSingle();
            RegisterTickableComponents();
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


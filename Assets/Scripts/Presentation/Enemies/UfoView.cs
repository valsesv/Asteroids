using UnityEngine;
using Zenject;
using Asteroids.Core.Enemies;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;
using Asteroids.Presentation.Player;

namespace Asteroids.Presentation.Enemies
{
    public class UfoView : EnemyView
    {
        [Inject]
        public void Construct(
            SignalBus signalBus,
            ScreenBounds screenBounds,
            ShipView shipView,
            EnemySettings enemySettings,
            EnemyFactory enemyFactory)
        {
            Vector2 position = new Vector2(transform.position.x, transform.position.y);
            float rotation = transform.eulerAngles.z;

            var playerTransform = shipView.Entity.GetComponent<TransformComponent>();

            Entity = enemyFactory.CreateUfo(position, rotation, signalBus, playerTransform, enemySettings, screenBounds);
            RegisterTickableComponents();
            _container.BindInstance(Entity).AsSingle();
        }

        public override void HandleEnemyDeath()
        {
            base.HandleEnemyDeath();

            _enemySpawner.ReturnEnemy(this);
        }
    }
}


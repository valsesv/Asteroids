using UnityEngine;
using Zenject;
using Asteroids.Core.Enemies;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;
using Asteroids.Presentation.Player;

namespace Asteroids.Presentation.Enemies
{
    /// <summary>
    /// UFO view that creates GameEntity using UfoFactory
    /// </summary>
    public class UfoView : EnemyView
    {
        [Inject]
        public void Construct(
            SignalBus signalBus,
            ScreenBounds screenBounds,
            ShipView shipView,
            EnemySettings enemySettings)
        {
            // Get position and rotation from Unity transform
            Vector2 position = new Vector2(transform.position.x, transform.position.y);
            float rotation = transform.eulerAngles.z;

            var playerTransform = shipView.Entity.GetComponent<TransformComponent>();

            // Create base enemy entity using static UfoFactory (no health needed)
            Entity = UfoFactory.CreateUfo(position, rotation, signalBus, playerTransform, enemySettings, screenBounds);
            RegisterTickableComponents();
            _container.BindInstance(Entity).AsSingle();
        }

        protected override void HandleEnemyDeath()
        {
            base.HandleEnemyDeath();

            // Return UFO to pool
            _enemySpawner.ReturnEnemy(this);
        }
    }
}


using UnityEngine;
using Zenject;
using Asteroids.Core.Enemies;
using Asteroids.Core.Entity;

namespace Asteroids.Presentation.Enemies
{
    /// <summary>
    /// Fragment view that creates GameEntity using FragmentFactory
    /// Fragments are spawned when asteroids are destroyed by bullets
    /// </summary>
    public class FragmentView : EnemyView
    {
        [Inject]
        public void Construct(
            SignalBus signalBus,
            ScreenBounds screenBounds,
            EnemySettings enemySettings)
        {
            // Get position and rotation from Unity transform
            Vector2 position = new Vector2(transform.position.x, transform.position.y);
            float rotation = transform.eulerAngles.z;

            // Create base enemy entity using static FragmentFactory (no health needed)
            Entity = FragmentFactory.CreateFragment(position, rotation, signalBus, enemySettings.FragmentSpeed, screenBounds);

            // Register Entity in container
            _container.BindInstance(Entity).AsSingle();
        }

        /// <summary>
        /// Set movement direction - used when spawning fragment
        /// </summary>
        public void SetDirection(Vector2 direction)
        {
            if (Entity != null)
            {
                var movement = Entity.GetComponent<AsteroidMovement>();
                if (movement != null)
                {
                    movement.SetDirection(direction);
                }
            }
        }

        /// <summary>
        /// Handle fragment death - return to pool (fragments don't fragment further)
        /// </summary>
        protected override void HandleEnemyDeath()
        {
            base.HandleEnemyDeath();

            // Return fragment to pool
            _enemySpawner.ReturnEnemy(this);
        }
    }
}


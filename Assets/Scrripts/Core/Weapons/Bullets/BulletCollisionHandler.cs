using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;
using Asteroids.Core.Enemies;
using Zenject;

namespace Asteroids.Core.Player
{
    /// <summary>
    /// Component that handles bullet collisions with enemies
    /// </summary>
    public class BulletCollisionHandler : IComponent
    {
        private readonly GameEntity _bulletEntity;
        private readonly SignalBus _signalBus;

        public BulletCollisionHandler(GameEntity bulletEntity, SignalBus signalBus)
        {
            _bulletEntity = bulletEntity;
            _signalBus = signalBus;
        }

        /// <summary>
        /// Handle collision with enemy
        /// </summary>
        public void HandleCollision(GameEntity enemyEntity)
        {
            var enemyComponent = enemyEntity.GetComponent<EnemyComponent>();
            if (enemyComponent == null)
            {
                return;
            }

            // Destroy bullet
            _signalBus?.Fire(new BulletDestroyedSignal { Entity = _bulletEntity });

            // Destroy enemy (asteroid or UFO - no fragmentation for now)
            _signalBus?.Fire(new EnemyDestroyedSignal { Entity = enemyEntity });
        }
    }
}


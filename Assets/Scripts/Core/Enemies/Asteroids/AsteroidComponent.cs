using Asteroids.Core.Entity;
using Zenject;

namespace Asteroids.Core.Enemies
{
    public class AsteroidComponent : EnemyComponent
    {
        public AsteroidComponent() : base(EnemyType.Asteroid)
        {
        }

        public override void Initialize(GameEntity entity, DiContainer container)
        {
            base.Initialize(entity, container);

            var enemySettings = container.Resolve<EnemySettings>();
            var movement = new AsteroidMovement(entity, enemySettings.AsteroidSpeed);
            entity.AddComponent(movement);
        }
    }
}
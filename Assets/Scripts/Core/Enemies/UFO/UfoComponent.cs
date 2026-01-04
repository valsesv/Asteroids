using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;
using Zenject;

namespace Asteroids.Core.Enemies
{
    public class UfoComponent : EnemyComponent
    {
        public UfoComponent() : base(EnemyType.Ufo)
        {
        }

        public override void Initialize(GameEntity entity, DiContainer container)
        {
            base.Initialize(entity, container);

            var playerEntity = container.Resolve<GameEntity>();
            var playerTransform = playerEntity.GetComponent<TransformComponent>();
            var enemySettings = container.Resolve<EnemySettings>();
            var movement = new UfoMovement(entity, playerTransform, enemySettings.UfoSpeed);
            entity.AddComponent(movement);
        }
    }
}
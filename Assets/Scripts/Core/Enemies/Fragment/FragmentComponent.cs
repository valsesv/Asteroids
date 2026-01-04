using Asteroids.Core.Entity;
using Zenject;

namespace Asteroids.Core.Enemies
{
    public class FragmentComponent : EnemyComponent
    {
        public FragmentComponent() : base(EnemyType.Fragment)
        {
        }

        public override void Initialize(GameEntity entity, DiContainer container)
        {
            base.Initialize(entity, container);

            var enemySettings = container.Resolve<EnemySettings>();
            var movement = new AsteroidMovement(entity, enemySettings.FragmentSpeed);
            entity.AddComponent(movement);
        }
    }
}
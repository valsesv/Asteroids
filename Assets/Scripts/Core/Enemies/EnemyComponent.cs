using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;
using Zenject;

namespace Asteroids.Core.Enemies
{
    public class EnemyComponent : IInitializableComponent
    {
        public EnemyType Type { get; private set; }

        public EnemyComponent(EnemyType type)
        {
            Type = type;
        }

        public virtual void Initialize(GameEntity entity, DiContainer container)
        {
            var screenBounds = container.Resolve<ScreenBounds>();
            var transformComponent = entity.GetComponent<TransformComponent>();
            var screenWrap = new ScreenWrapComponent(transformComponent, screenBounds);
            entity.AddComponent(screenWrap);
        }
    }
}


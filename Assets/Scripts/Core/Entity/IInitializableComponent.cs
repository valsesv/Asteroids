using Zenject;

namespace Asteroids.Core.Entity
{
    public interface IInitializableComponent : IComponent
    {
        void Initialize(GameEntity entity, DiContainer container);
    }
}
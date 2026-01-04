using Zenject;

namespace Asteroids.Core.Entity
{
    public interface ITickableComponent : IComponent, ITickable
    {
    }
}
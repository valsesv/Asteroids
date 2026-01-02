using Zenject;

namespace Asteroids.Core.Entity
{
    /// <summary>
    /// Interface for components that need to update each frame
    /// Combines IComponent and ITickable for semantic clarity
    /// </summary>
    public interface ITickableComponent : IComponent, ITickable
    {
    }
}


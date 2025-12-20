using Asteroids.Core.Entity.Components;

namespace Asteroids.Core.Enemies
{
    /// <summary>
    /// Component for UFO-specific data and behavior
    /// </summary>
    public class UfoComponent : EnemyComponent
    {
        public UfoComponent() : base(EnemyType.Ufo)
        {
        }
    }
}


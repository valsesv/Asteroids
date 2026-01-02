using Asteroids.Core.Entity;

namespace Asteroids.Core.Enemies
{
    /// <summary>
    /// Component that identifies an entity as an enemy
    /// Contains common enemy data and behavior
    /// </summary>
    public class EnemyComponent : IComponent
    {
        public EnemyType Type { get; private set; }

        public EnemyComponent(EnemyType type)
        {
            Type = type;
        }
    }
}


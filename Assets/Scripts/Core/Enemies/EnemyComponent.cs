using Asteroids.Core.Entity;

namespace Asteroids.Core.Enemies
{
    public class EnemyComponent : IComponent
    {
        public EnemyType Type { get; private set; }

        public EnemyComponent(EnemyType type)
        {
            Type = type;
        }
    }
}


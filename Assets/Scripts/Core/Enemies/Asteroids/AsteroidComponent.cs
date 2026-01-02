namespace Asteroids.Core.Enemies
{
    public class AsteroidComponent : EnemyComponent
    {
        public AsteroidComponent() : base(EnemyType.Asteroid)
        {
        }

        public int GetFragmentCount()
        {
            return 2;
        }
    }
}


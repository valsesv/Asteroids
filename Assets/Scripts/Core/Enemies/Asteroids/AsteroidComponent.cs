namespace Asteroids.Core.Enemies
{
    /// <summary>
    /// Component for asteroid-specific data and behavior
    /// Asteroids always fragment into Fragment enemies when destroyed by bullets
    /// </summary>
    public class AsteroidComponent : EnemyComponent
    {
        public AsteroidComponent() : base(EnemyType.Asteroid)
        {
        }

        /// <summary>
        /// Get the number of fragments to create when asteroid is destroyed
        /// </summary>
        public int GetFragmentCount()
        {
            return 2; // Always create 2 fragments
        }
    }
}


namespace Asteroids.Core.Enemies
{
    /// <summary>
    /// Component for asteroid-specific data and behavior
    /// </summary>
    public class AsteroidComponent : EnemyComponent
    {
        public AsteroidSize Size { get; private set; }

        public AsteroidComponent(AsteroidSize size) : base(EnemyType.Asteroid)
        {
            Size = size;
        }

        /// <summary>
        /// Check if asteroid can fragment (when hit by bullets)
        /// </summary>
        public bool CanFragment()
        {
            return Size != AsteroidSize.Small;
        }

        /// <summary>
        /// Get the size of fragments that should be created when this asteroid fragments
        /// </summary>
        public AsteroidSize GetFragmentSize()
        {
            return Size switch
            {
                AsteroidSize.Large => AsteroidSize.Medium,
                AsteroidSize.Medium => AsteroidSize.Small,
                _ => AsteroidSize.Small
            };
        }

        /// <summary>
        /// Get the number of fragments to create
        /// </summary>
        public int GetFragmentCount()
        {
            return Size switch
            {
                AsteroidSize.Large => 2,
                AsteroidSize.Medium => 2,
                _ => 0
            };
        }
    }
}


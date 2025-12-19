namespace Asteroids.Core.Enemies
{
    /// <summary>
    /// Asteroid model - contains asteroid-specific data
    /// Laser destroys immediately, bullets cause fragmentation (or destruction if smallest)
    /// </summary>
    public class AsteroidModel : EnemyModel
    {
        public AsteroidSize Size { get; set; }

        public AsteroidModel(AsteroidSize size) : base(EnemyType.Asteroid)
        {
            Size = size;
            Type = EnemyType.Asteroid;
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


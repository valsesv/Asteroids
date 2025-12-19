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
    }
}


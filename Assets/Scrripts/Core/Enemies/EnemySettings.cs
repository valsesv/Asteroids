namespace Asteroids.Core.Enemies
{
    /// <summary>
    /// Enemy settings - domain model
    /// Can be loaded from JSON via Infrastructure.JsonLoader
    /// </summary>
    [System.Serializable]
    public class EnemySettings
    {
        public float AsteroidSpeed = 3f;
        public float UfoSpeed = 4f;
        public int MaxEnemiesOnMap = 10;
        public float AsteroidSpawnWeight = 7f;
        public float UfoSpawnWeight = 3f;
    }
}


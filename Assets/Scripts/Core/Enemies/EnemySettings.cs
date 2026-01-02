namespace Asteroids.Core.Enemies
{
    [System.Serializable]
    public class EnemySettings
    {
        public float AsteroidSpeed = 3f;
        public float UfoSpeed = 4f;
        public float FragmentSpeed = 5f;
        public int MaxEnemiesOnMap = 10;
        public float AsteroidSpawnWeight = 7f;
        public float UfoSpawnWeight = 3f;
        public float SpawnInterval = 3f;
        public float SpawnDistance = 2f;
    }
}


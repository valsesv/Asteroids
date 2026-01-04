namespace Asteroids.Core.Enemies
{
    [System.Serializable]
    public class EnemySettings
    {
        public float AsteroidSpeed;
        public float UfoSpeed;
        public float FragmentSpeed;
        public int MaxEnemiesOnMap;
        public float AsteroidSpawnWeight;
        public float UfoSpawnWeight;
        public float SpawnInterval;
        public float SpawnDistance;
        public int AsteroidFragmentCount;
    }
}
namespace Asteroids.Core.Enemies
{
    /// <summary>
    /// Asteroid size determines fragmentation behavior
    /// </summary>
    public enum AsteroidSize
    {
        Large,   // Can fragment into Medium
        Medium,  // Can fragment into Small
        Small    // Cannot fragment, just destroyed
    }
}


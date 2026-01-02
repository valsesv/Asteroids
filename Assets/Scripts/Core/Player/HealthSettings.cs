namespace Asteroids.Core.Player
{
    /// <summary>
    /// Health settings - domain model
    /// Can be loaded from JSON via Infrastructure.JsonLoader
    /// </summary>
    [System.Serializable]
    public class HealthSettings
    {
        public float MaxHealth = 3f;
        public float InvincibilityDuration = 3f;
        public float BounceForce = 5f;
    }
}


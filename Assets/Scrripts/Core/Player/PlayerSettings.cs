namespace Asteroids.Core.Player
{
    /// <summary>
    /// Player settings - domain model
    /// Can be loaded from JSON via Infrastructure.JsonLoader
    /// </summary>
    public class PlayerSettings
    {
        public MovementSettings Movement;
        public StartPositionSettings StartPosition;
        public HealthSettings Health;
    }
}


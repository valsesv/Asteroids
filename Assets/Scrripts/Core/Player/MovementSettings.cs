namespace Asteroids.Core.Player
{
    public partial class ShipMovement
    {
        [System.Serializable]
        public class MovementSettings
        {
            public float Acceleration = 10f;
            public float MaxSpeed = 5f;
            public float Friction = 0.98f;
        }
    }
}


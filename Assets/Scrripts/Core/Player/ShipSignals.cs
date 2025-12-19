namespace Asteroids.Core.Signals
{
    /// <summary>
    /// Signal fired when ship position changes
    /// </summary>
    public class ShipPositionChangedSignal
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Rotation { get; set; }
    }

    /// <summary>
    /// Signal fired when ship velocity changes
    /// </summary>
    public class ShipVelocityChangedSignal
    {
        public float VelocityX { get; set; }
        public float VelocityY { get; set; }
        public float Speed { get; set; }
    }
}


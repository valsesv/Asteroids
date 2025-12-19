using UnityEngine;

namespace Asteroids.Core.Player
{
    /// <summary>
    /// Ship model - contains all ship data and state
    /// </summary>
    public class ShipModel
    {
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public Vector2 Velocity { get; set; }
        public bool CanControl { get; set; }

        public ShipModel()
        {
            CanControl = true;
        }
    }
}


using Asteroids.Core.Entity;

namespace Asteroids.Core.Player
{
    /// <summary>
    /// Component that identifies an entity as a ship
    /// Contains common ship data and behavior
    /// </summary>
    public class ShipComponent : IComponent
    {
        public bool CanControl { get; set; }

        public ShipComponent()
        {
            CanControl = true;
        }
    }
}


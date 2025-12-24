using Asteroids.Core.Entity;

namespace Asteroids.Core.Player
{
    /// <summary>
    /// Component that identifies an entity as a ship
    /// Contains common ship data and behavior
    /// </summary>
    public class ShipComponent : IComponent
    {
        private GameEntity _entity;
        private bool _canControl = true;

        public bool CanControl
        {
            get => _canControl;
            set
            {
                if (_canControl == value)
                {
                    return;
                }

                _canControl = value;

                // Sync weapon shooting with control state
                if (_entity != null)
                {
                    var weaponShooting = _entity.GetComponent<WeaponShooting>();
                    if (weaponShooting != null)
                    {
                        weaponShooting.CanShooting = value;
                    }
                }
            }
        }

        public ShipComponent(GameEntity entity)
        {
            _entity = entity;
            CanControl = true;
        }
    }
}


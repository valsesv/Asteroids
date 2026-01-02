using Asteroids.Core.Entity;

namespace Asteroids.Core.Player
{
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

                if (_entity == null)
                {
                    return;
                }
                var weaponShooting = _entity.GetComponent<WeaponShooting>();
                weaponShooting.CanShooting = value;
            }
        }

        public ShipComponent(GameEntity entity)
        {
            _entity = entity;
            CanControl = true;
        }
    }
}


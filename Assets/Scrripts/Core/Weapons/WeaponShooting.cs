using UnityEngine;
using Zenject;
using Asteroids.Core.PlayerInput;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;

namespace Asteroids.Core.Player
{
    /// <summary>
    /// Component that handles all weapon shooting (bullets and laser)
    /// </summary>
    public class WeaponShooting : ITickableComponent
    {
        private readonly TransformComponent _transform;
        private readonly IInputProvider _inputProvider;
        private readonly SignalBus _signalBus;
        private readonly BulletShootingLogic _bulletShooting;
        private readonly LaserShootingLogic _laserShooting;

        public bool CanShooting { get; set; } = true;

        public WeaponShooting(
            GameEntity entity,
            IInputProvider inputProvider,
            WeaponSettings weaponSettings,
            SignalBus signalBus,
            BulletFactory bulletFactory,
            LaserFactory laserFactory)
        {
            _transform = entity.GetComponent<TransformComponent>();
            _inputProvider = inputProvider;
            _signalBus = signalBus;

            // Create shooting logic classes (not components)
            _bulletShooting = new BulletShootingLogic(_transform, inputProvider, weaponSettings.Bullet, signalBus, bulletFactory);
            _laserShooting = new LaserShootingLogic(_transform, inputProvider, weaponSettings.Laser, signalBus, laserFactory);
        }

        public void Tick()
        {
            if (!CanShooting)
            {
                return;
            }

            // Try to shoot bullets
            _bulletShooting.TryShoot();

            // Try to shoot laser
            _laserShooting.TryShoot();
        }

        public int GetLaserCharges()
        {
            return _laserShooting.CurrentCharges;
        }

        public int GetMaxLaserCharges()
        {
            return _laserShooting.MaxCharges;
        }
    }
}


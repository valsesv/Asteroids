using UnityEngine;
using Zenject;
using Asteroids.Core.PlayerInput;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;
using Asteroids.Core.Weapons;

namespace Asteroids.Core.Player
{
    /// <summary>
    /// Component that handles weapon shooting (bullets and laser)
    /// </summary>
    public class WeaponShooting : ITickableComponent
    {
        private readonly BulletShootingLogic _bulletShooting;
        private readonly LaserShootingLogic _laserShooting;

        public bool CanShooting { get; set; } = true;

        public WeaponShooting(
            GameEntity entity,
            IInputProvider inputProvider,
            WeaponSettings weaponSettings,
            SignalBus signalBus)
        {
            var transform = entity.GetComponent<TransformComponent>();

            // Create shooting logic classes (not components)
            _bulletShooting = new BulletShootingLogic(transform, inputProvider, weaponSettings.Bullet, signalBus);
            
            // Get laser component (should be added to entity in ShipFactory)
            var laserComponent = entity.GetComponent<LaserComponent>();
            if (laserComponent != null)
            {
                _laserShooting = new LaserShootingLogic(transform, inputProvider, weaponSettings.Laser, laserComponent, signalBus);
            }
        }

        public void Tick()
        {
            if (!CanShooting)
            {
                return;
            }

            // Try to shoot bullets
            _bulletShooting.TryShoot();
            _laserShooting.TryShoot();
        }
    }
}


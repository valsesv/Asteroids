using Zenject;
using Asteroids.Core.PlayerInput;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;
using Asteroids.Core.Weapons;

namespace Asteroids.Core.Player
{
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

            _bulletShooting = new BulletShootingLogic(transform, inputProvider, weaponSettings.Bullet, signalBus);
            
            var laserComponent = entity.GetComponent<LaserComponent>();
            _laserShooting = new LaserShootingLogic(transform, inputProvider, laserComponent, signalBus);
        }

        public void Tick()
        {
            if (!CanShooting)
            {
                return;
            }

            _bulletShooting.TryShoot();
            _laserShooting.TryShoot();
        }
    }
}


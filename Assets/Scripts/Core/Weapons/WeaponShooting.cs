using Zenject;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;

namespace Asteroids.Core.Player
{
    public class WeaponShooting : ITickableComponent, IInitializable
    {
        private readonly BulletShootingLogic _bulletShooting;
        private readonly LaserShootingLogic _laserShooting;
        private readonly GameEntity _entity;

        public bool CanShooting { get; set; }

        public WeaponShooting(
            GameEntity entity,
            BulletShootingLogic bulletShooting,
            LaserShootingLogic laserShooting)
        {
            _entity = entity;
            _bulletShooting = bulletShooting;
            _laserShooting = laserShooting;
        }

        public void Initialize()
        {
            var transform = _entity.GetComponent<TransformComponent>();
            var laserComponent = _entity.GetComponent<LaserComponent>();

            _bulletShooting.Init(transform);
            _laserShooting.Init(transform, laserComponent);
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
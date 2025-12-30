using UnityEngine;
using Zenject;
using Asteroids.Core.PlayerInput;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;

namespace Asteroids.Core.Player
{
    /// <summary>
    /// Component that handles weapon shooting (bullets)
    /// </summary>
    public class WeaponShooting : ITickableComponent
    {
        private readonly BulletShootingLogic _bulletShooting;

        public bool CanShooting { get; set; } = true;

        public WeaponShooting(
            GameEntity entity,
            IInputProvider inputProvider,
            WeaponSettings weaponSettings,
            SignalBus signalBus)
        {
            var transform = entity.GetComponent<TransformComponent>();

            // Create shooting logic class (not component)
            _bulletShooting = new BulletShootingLogic(transform, inputProvider, weaponSettings.Bullet, signalBus);
        }

        public void Tick()
        {
            if (!CanShooting)
            {
                return;
            }

            // Try to shoot bullets
            _bulletShooting.TryShoot();
        }
    }
}


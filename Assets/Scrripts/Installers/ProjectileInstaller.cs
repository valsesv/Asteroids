using UnityEngine;
using Zenject;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;

namespace Asteroids.Installers
{
    /// <summary>
    /// Installer for projectile entities (bullets)
    /// Creates subcontainer for each projectile to track its signals
    /// </summary>
    public class ProjectileInstaller : InstallerBase
    {
        private readonly GameEntity _entity;

        public ProjectileInstaller(GameEntity entity)
        {
            _entity = entity;
        }

        public override void InstallBindings()
        {
            InstallSignals();
            InstallEntity();
            InstallTickableComponents();
        }

        private void InstallSignals()
        {
            // Declare signals for this projectile's subcontainer (like EnemyInstaller does)
            Container.DeclareSignal<TransformChangedSignal>();
            Container.DeclareSignal<PhysicsChangedSignal>();
        }

        private void InstallEntity()
        {
            // Bind the Entity for this projectile
            Container.BindInstance(_entity).AsSingle();
        }

        private void InstallTickableComponents()
        {
            // Bind ITickable components so they can be resolved and added to TickableManager
            foreach (var tickableComponent in _entity.GetTickableComponents())
            {
                Container.Bind<ITickable>()
                    .FromInstance(tickableComponent)
                    .AsSingle();
            }
        }
    }
}


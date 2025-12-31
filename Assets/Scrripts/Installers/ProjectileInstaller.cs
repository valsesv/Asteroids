using UnityEngine;
using Zenject;
using Asteroids.Presentation.Player;
using Asteroids.Core.Entity.Components;
using UnityEngine.Assertions;

namespace Asteroids.Installers
{
    /// <summary>
    /// Installer for projectile entities (bullets)
    /// </summary>
    public class ProjectileInstaller : MonoInstaller
    {
        [SerializeField] private BulletView _bulletViewPrefab;

        public override void InstallBindings()
        {
            AssertBulletViewPrefab();
            InstallSignals();
            InstallBulletView();
        }

        private void AssertBulletViewPrefab()
        {
            Assert.IsNotNull(_bulletViewPrefab, "BulletViewPrefab is not assigned in ProjectileInstaller!");
        }

        private void InstallSignals()
        {
            Container.DeclareSignal<TransformChangedSignal>();
            Container.DeclareSignal<PhysicsChangedSignal>();
            Container.DeclareSignal<BulletDestroyedSignal>();
        }

        private void InstallBulletView()
        {
            Container.Bind<BulletView>().FromInstance(_bulletViewPrefab).AsSingle();
            Container.BindInterfacesTo<BulletView>().FromInstance(_bulletViewPrefab);
        }
    }
}


using UnityEngine;
using Zenject;
using Asteroids.Presentation.Player;
using Asteroids.Core.Entity.Components;
using UnityEngine.Assertions;

namespace Asteroids.Installers
{
    public class ProjectileInstaller : MonoInstaller
    {
        [SerializeField] private BulletPresentation _bulletPresentationPrefab;

        public override void InstallBindings()
        {
            AssertBulletViewPrefab();
            InstallSignals();
            InstallBulletView();
        }

        private void AssertBulletViewPrefab()
        {
            Assert.IsNotNull(_bulletPresentationPrefab, "BulletPresentationPrefab is not assigned in ProjectileInstaller!");
        }

        private void InstallSignals()
        {
        }

        private void InstallBulletView()
        {
            Container.Bind<BulletPresentation>().FromInstance(_bulletPresentationPrefab).AsSingle();
            Container.BindInterfacesTo<BulletPresentation>().FromInstance(_bulletPresentationPrefab);
        }
    }
}


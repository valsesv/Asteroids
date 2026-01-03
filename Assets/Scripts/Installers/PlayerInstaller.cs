using UnityEngine;
using Zenject;
using Asteroids.Core.Entity;
using Asteroids.Core.Player;
using Asteroids.Presentation.Player;
using Asteroids.Core.Entity.Components;
using UnityEngine.Assertions;

namespace Asteroids.Installers
{
    public class PlayerInstaller : MonoInstaller
    {
        [SerializeField] private ShipPresentation _shipPresentationPrefab;

        public override void InstallBindings()
        {
            AssertShipViewPrefab();
            InstallTickableComponents();
            InstallShipView();
        }

        private void AssertShipViewPrefab()
        {
            Assert.IsNotNull(_shipPresentationPrefab, "ShipPresentationPrefab is not assigned in PlayerInstaller!");
        }

        private void InstallTickableComponents()
        {
            Container.Bind<ITickable>()
                .To<PhysicsComponent>()
                .FromMethod(ctx => ctx.Container.Resolve<GameEntity>().GetComponent<PhysicsComponent>())
                .AsSingle();

            Container.Bind<ITickable>()
                .To<ShipMovement>()
                .FromMethod(ctx => ctx.Container.Resolve<GameEntity>().GetComponent<ShipMovement>())
                .AsSingle();

            Container.Bind<ITickable>()
                .To<ScreenWrapComponent>()
                .FromMethod(ctx => ctx.Container.Resolve<GameEntity>().GetComponent<ScreenWrapComponent>())
                .AsSingle();

            Container.Bind<ITickable>()
                .To<WeaponShooting>()
                .FromMethod(ctx => ctx.Container.Resolve<GameEntity>().GetComponent<WeaponShooting>())
                .AsSingle();

            Container.Bind<ITickable>()
                .To<LaserComponent>()
                .FromMethod(ctx => ctx.Container.Resolve<GameEntity>().GetComponent<LaserComponent>())
                .AsSingle();
        }

        private void InstallShipView()
        {
            Container.Bind<ShipPresentation>().FromInstance(_shipPresentationPrefab).AsSingle();
            Container.BindInterfacesTo<ShipPresentation>().FromInstance(_shipPresentationPrefab);
        }
    }
}


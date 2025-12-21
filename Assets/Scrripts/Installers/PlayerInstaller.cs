using UnityEngine;
using Zenject;
using Asteroids.Core.Player;
using Asteroids.Presentation.Player;
using Asteroids.Core.Entity.Components;
using UnityEngine.Assertions;

namespace Asteroids.Installers
{
    public class PlayerInstaller : MonoInstaller
    {
        [SerializeField] private ShipView _shipViewPrefab;

        public override void InstallBindings()
        {
            AssertShipViewPrefab();
            InstallSignals();
            InstallShipModel();
            InstallTickableComponents();
            InstallShipView();
        }

        private void AssertShipViewPrefab()
        {
            Assert.IsNotNull(_shipViewPrefab, "ShipViewPrefab is not assigned in PlayerInstaller!");
        }

        private void InstallSignals()
        {
            Container.DeclareSignal<TransformChangedSignal>();
            Container.DeclareSignal<PhysicsChangedSignal>();
        }

        private void InstallShipModel()
        {
            Container.Bind<ShipModel>().AsSingle();
        }

        private void InstallTickableComponents()
        {
            // Bind ITickableComponents as ITickable so they update automatically
            // These components are created by ShipModel, so we resolve them from there
            Container.Bind<ITickable>()
                .To<PhysicsComponent>()
                .FromMethod(ctx => ctx.Container.Resolve<ShipModel>().GetComponent<PhysicsComponent>())
                .AsSingle();

            Container.Bind<ITickable>()
                .To<ShipMovement>()
                .FromMethod(ctx => ctx.Container.Resolve<ShipModel>().GetComponent<ShipMovement>())
                .AsSingle();

            Container.Bind<ITickable>()
                .To<ScreenWrapComponent>()
                .FromMethod(ctx => ctx.Container.Resolve<ShipModel>().GetComponent<ScreenWrapComponent>())
                .AsSingle();
        }

        private void InstallShipView()
        {
            Container.Bind<ShipView>().FromInstance(_shipViewPrefab).AsSingle();
            // ShipView implements IInitializable and IDisposable for signal subscriptions
            Container.BindInterfacesTo<ShipView>().FromInstance(_shipViewPrefab);
        }
    }
}


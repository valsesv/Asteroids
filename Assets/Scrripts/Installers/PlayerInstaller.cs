using UnityEngine;
using Zenject;
using Asteroids.Core.Entity;
using Asteroids.Core.Player;
using Asteroids.Presentation.Player;
using Asteroids.Core.Entity.Components;
using UnityEngine.Assertions;
using Asteroids.Core.PlayerInput;

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
            SignalBusInstaller.Install(Container);
            Container.DeclareSignal<TransformChangedSignal>();
            Container.DeclareSignal<PhysicsChangedSignal>();
            Container.DeclareSignal<HealthChangedSignal>();
            Container.DeclareSignal<InvincibilityChangedSignal>();
        }

        private void InstallShipModel()
        {
            Container.Bind<GameEntity>()
                .FromMethod(ctx => ShipFactory.CreateShip(
                    ctx.Container.Resolve<StartPositionSettings>(),
                    ctx.Container.Resolve<MovementSettings>(),
                    ctx.Container.Resolve<HealthSettings>(),
                    ctx.Container.Resolve<SignalBus>(),
                    ctx.Container.Resolve<IInputProvider>(),
                    ctx.Container.Resolve<ScreenBounds>()))
                .AsSingle();
        }

        private void InstallTickableComponents()
        {
            // Bind ITickableComponents as ITickable so they update automatically
            // These components are created by ShipFactory, so we resolve them from the GameEntity
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
        }

        private void InstallShipView()
        {
            Container.Bind<ShipView>().FromInstance(_shipViewPrefab).AsSingle();
            // ShipView implements IInitializable and IDisposable for signal subscriptions
            Container.BindInterfacesTo<ShipView>().FromInstance(_shipViewPrefab);
        }
    }
}


using UnityEngine;
using Zenject;
using Asteroids.Core.Player;
using Asteroids.Presentation.Player;
using Asteroids.Core.Entity.Components;
using UnityEngine.Assertions;

namespace Asteroids.Installers
{
    /// <summary>
    /// PlayerInstaller - installs bindings for the player GameObject
    /// Should be used with GameObjectContext on the player GameObject
    /// </summary>
    public class PlayerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            InstallSignals();
            InstallShipModel();
            InstallTickableComponents();
            InstallShipView();
        }

        private void InstallSignals()
        {
            // Declare player-specific signals
            // These signals are separate from enemy signals declared in GameInstaller
            Container.DeclareSignal<TransformChangedSignal>();
            Container.DeclareSignal<PhysicsChangedSignal>();
        }

        private void InstallShipModel()
        {
            // ShipModel requires dependencies from parent container:
            // - StartPositionSettings
            // - MovementSettings
            // - SignalBus
            // - IInputProvider
            // - ScreenBounds
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
            // ShipView should be on the same GameObject as GameObjectContext
            // Try to get it from the current GameObject hierarchy
            var shipView = GetComponentInChildren<ShipView>();

            if (shipView == null)
            {
                // If not found, try to get it from the root GameObject
                shipView = GetComponent<ShipView>();
            }

            if (shipView != null)
            {
                Container.Bind<ShipView>().FromInstance(shipView).AsSingle();
                // ShipView implements IInitializable and IDisposable for signal subscriptions
                Container.BindInterfacesTo<ShipView>().FromInstance(shipView);
            }
            else
            {
                Debug.LogWarning("ShipView not found on player GameObject. Make sure ShipView component is attached to the same GameObject or its children.");
            }
        }
    }
}


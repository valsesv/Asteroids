using UnityEngine;
using Zenject;
using Asteroids.Core.Player;
using Asteroids.Presentation.Player;

namespace Asteroids.Installers
{
    /// <summary>
    /// GameInstaller - installs bindings for the game scene
    /// </summary>
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private ShipView _shipViewPrefab;

        public override void InstallBindings()
        {
            InstallSignals();
            InstallShip();
        }

        private void InstallSignals()
        {
            Container.DeclareSignal<Core.Signals.ShipPositionChangedSignal>();
            Container.DeclareSignal<Core.Signals.ShipVelocityChangedSignal>();
        }

        private void InstallShip()
        {
            // Ship Model - single instance
            Container.Bind<ShipModel>().AsSingle();

            // Ship Components
            Container.BindInterfacesAndSelfTo<ShipInitializer>().AsSingle();

            // Ship View - create from prefab or scene
            if (_shipViewPrefab != null)
            {
                Container.Bind<ShipView>().FromComponentInNewPrefab(_shipViewPrefab).AsSingle();
            }
            else
            {
                Container.Bind<ShipView>().FromComponentInHierarchy().AsSingle();
            }

            // ShipView implements IInitializable and IDisposable for signal subscriptions
            Container.BindInterfacesTo<ShipView>().FromResolve();
        }
    }
}

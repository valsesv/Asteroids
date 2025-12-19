using UnityEngine;
using Zenject;
using Asteroids.Core.Player;
using Asteroids.Core.PlayerInput;
using Asteroids.Presentation.Player;
using UnityEngine.Assertions;

namespace Asteroids.Installers
{
    /// <summary>
    /// GameInstaller - installs bindings for the game scene
    /// </summary>
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private ShipView _shipViewPrefab;
        [SerializeField] private KeyboardInputSettings _inputSettings;
        [SerializeField] private ShipMovement.MovementSettings _movementSettings;

        public override void InstallBindings()
        {
            InstallSignals();
            InstallInput();
            InstallShip();
        }

        private void InstallSignals()
        {
            Container.DeclareSignal<Core.Signals.ShipPositionChangedSignal>();
            Container.DeclareSignal<Core.Signals.ShipVelocityChangedSignal>();
        }

        private void InstallInput()
        {
            Assert.IsNotNull(_inputSettings, "KeyboardInputSettings is not assigned in GameInstaller! Please assign it in Inspector or create a ScriptableObject asset via: Create > Asteroids > Settings > Keyboard Input Settings");
            Container.BindInstance(_inputSettings);

            // Input Provider
            Container.Bind<IInputProvider>().To<KeyboardInputProvider>().AsSingle();
        }

        private void InstallShip()
        {
            Container.Bind<ShipModel>().AsSingle();

            Assert.IsNotNull(_movementSettings, "MovementSettings is not assigned in GameInstaller!");
            Container.BindInstance(_movementSettings);

            // Ship Components
            Container.BindInterfacesAndSelfTo<ShipInitializer>().AsSingle();
            Container.BindInterfacesAndSelfTo<ShipMovement>().AsSingle();

            Assert.IsNotNull(_shipViewPrefab, "ShipViewPrefab is not assigned in GameInstaller!");
            Container.Bind<ShipView>().FromComponentInNewPrefab(_shipViewPrefab).AsSingle();

            // ShipView implements IInitializable and IDisposable for signal subscriptions
            Container.BindInterfacesTo<ShipView>().FromResolve();
        }
    }
}

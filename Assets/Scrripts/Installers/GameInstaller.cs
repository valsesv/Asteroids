using UnityEngine;
using Zenject;
using Asteroids.Core.Player;
using Asteroids.Core.PlayerInput;
using Asteroids.Presentation.Player;
using UnityEngine.Assertions;
using Utils.JsonLoader;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;

namespace Asteroids.Installers
{
    /// <summary>
    /// GameInstaller - installs bindings for the game scene
    /// </summary>
    public class GameInstaller : MonoInstaller
    {
        private const string PlayerSettingsFileName = "player_settings.json";

        [SerializeField] private ShipView _shipViewPrefab;
        [SerializeField] private KeyboardInputSettings _inputSettings;

        public override void InstallBindings()
        {
            InstallSettings();
            InstallSignals();
            InstallInput();
            InstallShip();
        }

        private void InstallSettings()
        {
            // Load player settings from JSON
            var jsonLoader = new JsonLoader();
            var playerSettings = jsonLoader.LoadFromStreamingAssets<PlayerSettings>(PlayerSettingsFileName);

            Assert.IsNotNull(playerSettings, "Failed to load player settings from JSON!");
            Assert.IsNotNull(playerSettings.Movement, "Movement settings are null in player settings!");
            Assert.IsNotNull(playerSettings.StartPosition, "Start position settings are null in player settings!");

            // Bind settings directly from JSON
            Container.BindInstance(playerSettings.Movement);
            Container.BindInstance(playerSettings.StartPosition);
        }

        private void InstallSignals()
        {
            // Declare generic component signals (reusable for any entity)
            Container.DeclareSignal<TransformChangedSignal>();
            Container.DeclareSignal<PhysicsChangedSignal>();
        }

        private void InstallInput()
        {
            // Input Settings - ScriptableObject from Inspector
            Assert.IsNotNull(_inputSettings, "KeyboardInputSettings is not assigned in GameInstaller! Please assign it in Inspector or create a ScriptableObject asset via: Create > Asteroids > Settings > Keyboard Input Settings");
            Container.BindInstance(_inputSettings);

            // Input Provider
            Container.Bind<IInputProvider>().To<KeyboardInputProvider>().AsSingle();
        }

        private void InstallShip()
        {
            Container.Bind<ShipModel>().AsSingle();

            // Bind ITickableComponents as ITickable so they update automatically
            // Using ITickableComponent interface for type safety
            Container.Bind<ITickable>()
                .To<PhysicsComponent>()
                .FromMethod(ctx => ctx.Container.Resolve<ShipModel>().GetComponent<PhysicsComponent>())
                .AsSingle();

            Container.Bind<ITickable>()
                .To<ShipMovement>()
                .FromMethod(ctx => ctx.Container.Resolve<ShipModel>().GetComponent<ShipMovement>())
                .AsSingle();

            Assert.IsNotNull(_shipViewPrefab, "ShipViewPrefab is not assigned in GameInstaller!");
            Container.Bind<ShipView>().FromComponentInNewPrefab(_shipViewPrefab).AsSingle();

            // ShipView implements IInitializable and IDisposable for signal subscriptions
            Container.BindInterfacesTo<ShipView>().FromResolve();
        }
    }
}

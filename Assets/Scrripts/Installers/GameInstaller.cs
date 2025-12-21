using UnityEngine;
using Zenject;
using Asteroids.Core.Player;
using Asteroids.Core.PlayerInput;
using UnityEngine.Assertions;
using Utils.JsonLoader;
using Asteroids.Core.Entity;

namespace Asteroids.Installers
{
    /// <summary>
    /// GameInstaller - installs bindings for the game scene
    /// </summary>
    public class GameInstaller : MonoInstaller
    {
        private const string PlayerSettingsFileName = "player_settings.json";

        [SerializeField] private KeyboardInputSettings _inputSettings;

        public override void InstallBindings()
        {
            InstallSettings();
            InstallInput();
            InstallCommonServices();
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

        private void InstallInput()
        {
            // Input Settings - ScriptableObject from Inspector
            Assert.IsNotNull(_inputSettings, "KeyboardInputSettings is not assigned in GameInstaller! Please assign it in Inspector or create a ScriptableObject asset via: Create > Asteroids > Settings > Keyboard Input Settings");
            Container.BindInstance(_inputSettings);

            // Input Provider
            Container.Bind<IInputProvider>().To<KeyboardInputProvider>().AsSingle();
        }

        private void InstallCommonServices()
        {
            // Bind ScreenBounds service (uses main camera)
            // This is used by both player and enemies, so it's in the common services
            Container.Bind<ScreenBounds>()
                .FromMethod(ctx => new ScreenBounds(Camera.main))
                .AsSingle();
        }
    }
}

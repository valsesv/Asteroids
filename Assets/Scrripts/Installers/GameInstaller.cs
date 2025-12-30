using UnityEngine;
using Zenject;
using Asteroids.Core.Player;
using Asteroids.Core.PlayerInput;
using Asteroids.Core.Enemies;
using UnityEngine.Assertions;
using Utils.JsonLoader;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;
using Asteroids.Presentation.Player;
using Asteroids.Presentation.Enemies;

namespace Asteroids.Installers
{
    /// <summary>
    /// GameInstaller - installs bindings for the game scene
    /// </summary>
    public class GameInstaller : MonoInstaller
    {
        private const string PlayerSettingsFileName = "player_settings.json";
        private const string EnemySettingsFileName = "enemy_settings.json";

        [SerializeField] private KeyboardInputSettings _inputSettings;
        [SerializeField] private ShipView _shipViewPrefab;
        [SerializeField] private EnemySpawner _enemySpawner;
        [SerializeField] private ProjectileSpawner _projectileSpawner;

        public override void InstallBindings()
        {
            InstallSignalBus();
            InstallSettings();
            InstallInput();
            InstallCommonServices();
            InstallEnemySpawner();
        }

        private void InstallSignalBus()
        {
            SignalBusInstaller.Install(Container);

            // Declare all game signals here so they're available to all components
            // (bullets, enemies, player, etc.)
            Container.DeclareSignal<TransformChangedSignal>();
            Container.DeclareSignal<PhysicsChangedSignal>();
            Container.DeclareSignal<HealthChangedSignal>();
            Container.DeclareSignal<InvincibilityChangedSignal>();
            Container.DeclareSignal<BulletCreatedSignal>();
            Container.DeclareSignal<BulletShotSignal>();
            Container.DeclareSignal<BulletDestroyedSignal>();
            Container.DeclareSignal<EnemyDestroyedSignal>();
            // AsteroidFragmentSignal - временно отключено, добавим позже
        }

        private void InstallSettings()
        {
            var jsonLoader = new JsonLoader();

            // Load player settings
            var playerSettings = jsonLoader.LoadFromStreamingAssets<PlayerSettings>(PlayerSettingsFileName);
            Assert.IsNotNull(playerSettings, "Failed to load player settings from JSON!");
            Assert.IsNotNull(playerSettings.Movement, "Movement settings are null in player settings!");
            Assert.IsNotNull(playerSettings.StartPosition, "Start position settings are null in player settings!");
            Assert.IsNotNull(playerSettings.Health, "Health settings are null in player settings!");
            Assert.IsNotNull(playerSettings.Weapon, "Weapon settings are null in player settings!");
            Container.BindInstance(playerSettings.Movement);
            Container.BindInstance(playerSettings.StartPosition);
            Container.BindInstance(playerSettings.Health);
            Container.BindInstance(playerSettings.Weapon);
            Container.BindInstance(playerSettings.Weapon.Bullet);

            // Load enemy settings
            var enemySettings = jsonLoader.LoadFromStreamingAssets<EnemySettings>(EnemySettingsFileName);
            Assert.IsNotNull(enemySettings, "Failed to load enemy settings from JSON!");
            Container.BindInstance(enemySettings);
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

            Container.Bind<ShipView>().FromInstance(_shipViewPrefab).AsSingle();

            Assert.IsNotNull(_projectileSpawner, "ProjectileSpawner is not assigned in GameInstaller!");
            Container.BindInterfacesTo<ProjectileSpawner>().FromInstance(_projectileSpawner).AsSingle();
        }

        private void InstallEnemySpawner()
        {
            Assert.IsNotNull(_enemySpawner, "EnemySpawner is not assigned in GameInstaller!");
            Container.BindInterfacesAndSelfTo<EnemySpawner>().FromInstance(_enemySpawner).AsSingle();
        }
    }
}

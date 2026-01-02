using UnityEngine;
using Zenject;
using Asteroids.Core.Player;
using Asteroids.Core.PlayerInput;
using Asteroids.Core.Enemies;
using Asteroids.Core.Score;
using Asteroids.Core.Weapons;
using UnityEngine.Assertions;
using Utils.JsonLoader;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;
using Asteroids.Presentation.Player;
using Asteroids.Presentation.Enemies;
using Asteroids.Presentation.Effects;
using Asteroids.Presentation.UI;
using Asteroids.Core.Game;

namespace Asteroids.Installers
{
    /// <summary>
    /// GameInstaller - installs bindings for the game scene
    /// </summary>
    public class GameInstaller : MonoInstaller
    {
        private const string PlayerSettingsFileName = "player_settings.json";
        private const string EnemySettingsFileName = "enemy_settings.json";
        private const string ScoreSettingsFileName = "score_settings.json";

        [SerializeField] private KeyboardInputSettings _inputSettings;
        [SerializeField] private ShipView _shipViewPrefab;
        [SerializeField] private EnemySpawner _enemySpawner;
        [SerializeField] private ProjectileSpawner _projectileSpawner;
        [SerializeField] private ParticleEffectSpawner _particleEffectSpawner;
        [SerializeField] private LaserView _laserView;
        [SerializeField] private ScoreView _scoreView;
        [SerializeField] private HealthView _healthView;
        [SerializeField] private PlayerStatsView _playerStatsView;
        [SerializeField] private StartMenuView _startMenuView;
        [SerializeField] private GameUIView _gameUIView;

        [Header("Mobile Input (Optional)")]
        [SerializeField] private GameObject _mobileInputPanelPrefab;
        [SerializeField] private Canvas _gameCanvas;

        public override void InstallBindings()
        {
            InstallSignalBus();
            InstallSettings();
            InstallInput();
            InstallCommonServices();
            InstallPlayerEntity(); // Install player entity before GameController
            InstallEnemySpawner();
            InstallParticleEffectSpawner();
            InstallLaserView();
            InstallScoreUI();
            InstallHealthUI();
            InstallPlayerStatsUI();
            InstallGameController();
            InstallStartMenuUI();
            InstallGameUI();
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
            Container.DeclareSignal<EnemyDestroyedSignal>();
            Container.DeclareSignal<LaserShotSignal>();
            Container.DeclareSignal<LaserChargesChangedSignal>();
            Container.DeclareSignal<LaserDeactivatedSignal>();
            Container.DeclareSignal<ScoreChangedSignal>();
            Container.DeclareSignal<GameStartedSignal>();
            Container.DeclareSignal<GameOverSignal>();
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
            Assert.IsNotNull(playerSettings.Weapon.Laser, "Laser settings are null in weapon settings!");
            Container.BindInstance(playerSettings.Weapon.Laser);

            // Load enemy settings
            var enemySettings = jsonLoader.LoadFromStreamingAssets<EnemySettings>(EnemySettingsFileName);
            Assert.IsNotNull(enemySettings, "Failed to load enemy settings from JSON!");
            Container.BindInstance(enemySettings);

            // Load score settings
            var scoreSettings = jsonLoader.LoadFromStreamingAssets<ScoreSettings>(ScoreSettingsFileName);
            Assert.IsNotNull(scoreSettings, "Failed to load score settings from JSON!");
            scoreSettings.InitializeRewards();
            Container.BindInstance(scoreSettings);

            // Bind score service
            Container.BindInterfacesAndSelfTo<ScoreService>().AsSingle();
        }

        private void InstallInput()
        {
            // Input Settings - ScriptableObject from Inspector
            Assert.IsNotNull(_inputSettings, "KeyboardInputSettings is not assigned in GameInstaller! Please assign it in Inspector or create a ScriptableObject asset via: Create > Asteroids > Settings > Keyboard Input Settings");
            Container.BindInstance(_inputSettings);

            // Check if we should use mobile input (Editor or Android)
            bool shouldUseMobileInput = Application.isEditor || Application.platform == RuntimePlatform.Android;
            bool isEditor = Application.isEditor;

            VirtualJoystickView joystickView = null;
            MobileInputView mobileInputView = null;

            // Always bind keyboard input provider (for Editor and desktop)
            Container.Bind<KeyboardInputProvider>().AsSingle();

            // Instantiate mobile input panel prefab in GameCanvas if needed
            if (shouldUseMobileInput && _mobileInputPanelPrefab != null && _gameCanvas != null)
            {
                // Instantiate panel prefab as child of GameCanvas
                GameObject mobileInputInstance = Instantiate(_mobileInputPanelPrefab, _gameCanvas.transform);

                // Find components in the instantiated prefab
                joystickView = mobileInputInstance.GetComponentInChildren<VirtualJoystickView>();
                mobileInputView = mobileInputInstance.GetComponentInChildren<MobileInputView>();
            }

            // Choose input provider based on platform
            if (isEditor && joystickView != null && mobileInputView != null)
            {
                // Editor: Use combined input (both keyboard and mobile)
                Container.BindInstance(joystickView);
                Container.BindInstance(mobileInputView);
                Container.Bind<VirtualJoystickInputProvider>().AsSingle();

                Container.Bind<IInputProvider>()
                    .To<CombinedInputProvider>()
                    .AsSingle();
            }
            else if (shouldUseMobileInput && joystickView != null && mobileInputView != null)
            {
                // Android: Use only mobile input
                Container.BindInstance(joystickView);
                Container.BindInstance(mobileInputView);
                Container.Bind<IInputProvider>()
                    .To<VirtualJoystickInputProvider>()
                    .AsSingle();
            }
            else
            {
                // Desktop: Use only keyboard and mouse
                Container.Bind<IInputProvider>()
                    .To<KeyboardInputProvider>()
                    .AsSingle();
            }
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
            Container.BindInterfacesAndSelfTo<ProjectileSpawner>().FromInstance(_projectileSpawner).AsSingle();
        }

        private void InstallPlayerEntity()
        {
            // Install player entity - must be done before GameController
            Container.Bind<GameEntity>()
                .FromMethod(ctx => ShipFactory.CreateShip(
                    ctx.Container.Resolve<StartPositionSettings>(),
                    ctx.Container.Resolve<MovementSettings>(),
                    ctx.Container.Resolve<HealthSettings>(),
                    ctx.Container.Resolve<WeaponSettings>(),
                    ctx.Container.Resolve<SignalBus>(),
                    ctx.Container.Resolve<IInputProvider>(),
                    ctx.Container.Resolve<ScreenBounds>()))
                .AsSingle();
        }

        private void InstallEnemySpawner()
        {
            Assert.IsNotNull(_enemySpawner, "EnemySpawner is not assigned in GameInstaller!");
            Container.BindInterfacesAndSelfTo<EnemySpawner>().FromInstance(_enemySpawner).AsSingle();
        }

        private void InstallParticleEffectSpawner()
        {
            if (_particleEffectSpawner != null)
            {
                Container.BindInterfacesAndSelfTo<ParticleEffectSpawner>().FromInstance(_particleEffectSpawner).AsSingle();
            }
        }

        private void InstallLaserView()
        {
            if (_laserView != null)
            {
                Container.BindInterfacesAndSelfTo<LaserView>().FromInstance(_laserView).AsSingle();
            }
        }

        private void InstallScoreUI()
        {
            // Bind ScoreViewModel (MVVM pattern)
            Container.BindInterfacesAndSelfTo<ScoreViewModel>().AsSingle();

            // Bind ScoreView if assigned
            if (_scoreView != null)
            {
                Container.BindInterfacesAndSelfTo<ScoreView>().FromInstance(_scoreView).AsSingle();
            }
        }

        private void InstallHealthUI()
        {
            // Bind HealthViewModel (MVVM pattern)
            Container.BindInterfacesAndSelfTo<HealthViewModel>().AsSingle();
            Container.BindInterfacesAndSelfTo<HealthView>().FromInstance(_healthView).AsSingle();
        }

        private void InstallPlayerStatsUI()
        {
            // Bind PlayerStatsViewModel (MVVM pattern)
            Container.BindInterfacesAndSelfTo<PlayerStatsViewModel>().AsSingle();

            // Bind PlayerStatsView if assigned
            if (_playerStatsView != null)
            {
                Container.BindInterfacesAndSelfTo<PlayerStatsView>().FromInstance(_playerStatsView).AsSingle();
            }
        }

        private void InstallGameController()
        {
            // Bind GameController (non-MonoBehaviour)
            Container.BindInterfacesAndSelfTo<GameController>().AsSingle();
        }

        private void InstallStartMenuUI()
        {
            // Bind StartMenuViewModel (MVVM pattern)
            Container.BindInterfacesAndSelfTo<StartMenuViewModel>().AsSingle();

            // Bind StartMenuView if assigned
            if (_startMenuView != null)
            {
                Container.BindInterfacesAndSelfTo<StartMenuView>().FromInstance(_startMenuView).AsSingle();
            }
        }

        private void InstallGameUI()
        {
            // Bind GameUIView if assigned
            if (_gameUIView != null)
            {
                Container.BindInterfacesAndSelfTo<GameUIView>().FromInstance(_gameUIView).AsSingle();
            }
        }
    }
}

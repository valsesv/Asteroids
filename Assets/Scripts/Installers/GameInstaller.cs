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
    public class GameInstaller : MonoInstaller
    {
        private const string PlayerSettingsFileName = "player_settings.json";
        private const string EnemySettingsFileName = "enemy_settings.json";
        private const string ScoreSettingsFileName = "score_settings.json";

        [SerializeField] private KeyboardInputSettings _inputSettings;
        [SerializeField] private ShipPresentation _shipPresentationPrefab;
        [SerializeField] private EnemySpawner _enemySpawner;
        [SerializeField] private ProjectileSpawner _projectileSpawner;
        [SerializeField] private ParticleEffectSpawner _particleEffectSpawner;
        [SerializeField] private LaserPresentation _laserPresentation;
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
            InstallSignals();
            InstallSettings();
            InstallInput();
            InstallCommonServices();
            InstallWeaponShooting();
            InstallPlayerEntity();
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

        private void InstallSignals()
        {
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
        }

        private void InstallSettings()
        {
            var jsonLoader = new JsonLoader();

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

            var enemySettings = jsonLoader.LoadFromStreamingAssets<EnemySettings>(EnemySettingsFileName);
            Assert.IsNotNull(enemySettings, "Failed to load enemy settings from JSON!");
            Container.BindInstance(enemySettings);

            var scoreSettings = jsonLoader.LoadFromStreamingAssets<ScoreSettings>(ScoreSettingsFileName);
            Assert.IsNotNull(scoreSettings, "Failed to load score settings from JSON!");
            Container.BindInstance(scoreSettings);

            Container.BindInterfacesAndSelfTo<ScoreService>().AsSingle();
        }

        private void InstallInput()
        {
            Assert.IsNotNull(_inputSettings, "KeyboardInputSettings is not assigned in GameInstaller! Please assign it in Inspector or create a ScriptableObject asset via: Create > Asteroids > Settings > Keyboard Input Settings");
            Container.BindInstance(_inputSettings);

            bool shouldUseMobileInput = Application.isEditor || Application.platform == RuntimePlatform.Android;
            bool isEditor = Application.isEditor;

            VirtualJoystickView joystickView = null;
            MobileInputView mobileInputView = null;

            Container.Bind<KeyboardInputProvider>().AsSingle();

            if (shouldUseMobileInput && _mobileInputPanelPrefab != null && _gameCanvas != null)
            {
                GameObject mobileInputInstance = Instantiate(_mobileInputPanelPrefab, _gameCanvas.transform);

                joystickView = mobileInputInstance.GetComponentInChildren<VirtualJoystickView>();
                mobileInputView = mobileInputInstance.GetComponentInChildren<MobileInputView>();
            }

            if (isEditor && joystickView != null && mobileInputView != null)
            {
                Container.BindInstance(joystickView);
                Container.BindInstance(mobileInputView);
                Container.Bind<VirtualJoystickInputProvider>().AsSingle();

                Container.Bind<IInputProvider>()
                    .To<CombinedInputProvider>()
                    .AsSingle();
            }
            else if (shouldUseMobileInput && joystickView != null && mobileInputView != null)
            {
                Container.BindInstance(joystickView);
                Container.BindInstance(mobileInputView);
                Container.Bind<IInputProvider>()
                    .To<VirtualJoystickInputProvider>()
                    .AsSingle();
            }
            else
            {
                Container.Bind<IInputProvider>()
                    .To<KeyboardInputProvider>()
                    .AsSingle();
            }
        }

        private void InstallCommonServices()
        {
            Container.Bind<ScreenBounds>()
                .FromMethod(ctx => new ScreenBounds(Camera.main))
                .AsSingle();

            Container.Bind<EnemyFactory>().AsSingle();

            Container.Bind<ShipPresentation>().FromInstance(_shipPresentationPrefab).AsSingle();

            Assert.IsNotNull(_projectileSpawner, "ProjectileSpawner is not assigned in GameInstaller!");
            Container.BindInterfacesAndSelfTo<ProjectileSpawner>().FromInstance(_projectileSpawner).AsSingle();
        }

        private void InstallWeaponShooting()
        {
            Container.Bind<BulletShootingLogic>().AsSingle();
            Container.Bind<LaserShootingLogic>().AsSingle();
        }

        private void InstallPlayerEntity()
        {
            Container.Bind<GameEntity>()
                .FromMethod(ctx => ShipFactory.CreateShip(
                    ctx.Container.Resolve<StartPositionSettings>(),
                    ctx.Container.Resolve<MovementSettings>(),
                    ctx.Container.Resolve<HealthSettings>(),
                    ctx.Container.Resolve<WeaponSettings>(),
                    ctx.Container.Resolve<SignalBus>(),
                    ctx.Container.Resolve<IInputProvider>(),
                    ctx.Container.Resolve<ScreenBounds>(),
                    ctx.Container))
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
            if (_laserPresentation != null)
            {
                Container.BindInterfacesAndSelfTo<LaserPresentation>().FromInstance(_laserPresentation).AsSingle();
            }
        }

        private void InstallScoreUI()
        {
            Container.BindInterfacesAndSelfTo<ScoreViewModel>().AsSingle();

            if (_scoreView != null)
            {
                Container.BindInterfacesAndSelfTo<ScoreView>().FromInstance(_scoreView).AsSingle();
            }
        }

        private void InstallHealthUI()
        {
            Container.BindInterfacesAndSelfTo<HealthViewModel>().AsSingle();
            Container.BindInterfacesAndSelfTo<HealthView>().FromInstance(_healthView).AsSingle();
        }

        private void InstallPlayerStatsUI()
        {
            Container.BindInterfacesAndSelfTo<PlayerStatsViewModel>().AsSingle();

            if (_playerStatsView != null)
            {
                Container.BindInterfacesAndSelfTo<PlayerStatsView>().FromInstance(_playerStatsView).AsSingle();
            }
        }

        private void InstallGameController()
        {
            Container.BindInterfacesAndSelfTo<GameController>().AsSingle();
        }

        private void InstallStartMenuUI()
        {
            Container.BindInterfacesAndSelfTo<StartMenuViewModel>().AsSingle();

            if (_startMenuView != null)
            {
                Container.BindInterfacesAndSelfTo<StartMenuView>().FromInstance(_startMenuView).AsSingle();
            }
        }

        private void InstallGameUI()
        {
            if (_gameUIView != null)
            {
                Container.BindInterfacesAndSelfTo<GameUIView>().FromInstance(_gameUIView).AsSingle();
            }
        }
    }
}

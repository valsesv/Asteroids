using UnityEngine;
using Zenject;
using Asteroids.Core.Player;
using Asteroids.Core.PlayerInput;
using Asteroids.Core.Enemies;
using Asteroids.Core.Score;
using Asteroids.Core.Weapons;
using UnityEngine.Assertions;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;
using Asteroids.Presentation.Player;
using Asteroids.Presentation.Enemies;
using Asteroids.Presentation.Effects;
using Asteroids.Presentation.UI;
using Asteroids.Core.Game;
using Asteroids.Infrastructure;

namespace Asteroids.Installers
{
    public class GameInstaller : MonoInstaller
    {
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
            AssertRequiredFields();
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

        private void AssertRequiredFields()
        {
            Assert.IsNotNull(_inputSettings, "KeyboardInputSettings is not assigned in GameInstaller!");
            Assert.IsNotNull(_shipPresentationPrefab, "ShipPresentationPrefab is not assigned in GameInstaller!");
            Assert.IsNotNull(_enemySpawner, "EnemySpawner is not assigned in GameInstaller!");
            Assert.IsNotNull(_projectileSpawner, "ProjectileSpawner is not assigned in GameInstaller!");
            Assert.IsNotNull(_particleEffectSpawner, "ParticleEffectSpawner is not assigned in GameInstaller!");
            Assert.IsNotNull(_laserPresentation, "LaserPresentation is not assigned in GameInstaller!");
            Assert.IsNotNull(_scoreView, "ScoreView is not assigned in GameInstaller!");
            Assert.IsNotNull(_healthView, "HealthView is not assigned in GameInstaller!");
            Assert.IsNotNull(_playerStatsView, "PlayerStatsView is not assigned in GameInstaller!");
            Assert.IsNotNull(_startMenuView, "StartMenuView is not assigned in GameInstaller!");
            Assert.IsNotNull(_gameUIView, "GameUIView is not assigned in GameInstaller!");
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
            var settingsLoader = new GameSettingsLoader();
            var loadedSettings = settingsLoader.LoadSettings();

            Container.BindInstance(loadedSettings.PlayerSettings.Movement);
            Container.BindInstance(loadedSettings.PlayerSettings.StartPosition);
            Container.BindInstance(loadedSettings.PlayerSettings.Health);
            Container.BindInstance(loadedSettings.PlayerSettings.Weapon);
            Container.BindInstance(loadedSettings.PlayerSettings.Weapon.Bullet);
            Container.BindInstance(loadedSettings.PlayerSettings.Weapon.Laser);

            Container.BindInstance(loadedSettings.EnemySettings);
            Container.BindInstance(loadedSettings.ScoreSettings);

            Container.BindInterfacesAndSelfTo<ScoreService>().AsSingle();
        }

        private void InstallInput()
        {
            Container.BindInstance(_inputSettings);

            var inputSetup = new InputProviderSetup();
            var setupResult = inputSetup.Setup(_mobileInputPanelPrefab, _gameCanvas);

            Container.Bind<KeyboardInputProvider>().AsSingle();

            if (setupResult.JoystickView != null)
            {
                Container.BindInstance(setupResult.JoystickView);
            }

            if (setupResult.MobileInputView != null)
            {
                Container.BindInstance(setupResult.MobileInputView);
            }

            switch (setupResult.ProviderType)
            {
                case InputProviderType.Combined:
                    Container.Bind<VirtualJoystickInputProvider>().AsSingle();
                    Container.Bind<IInputProvider>()
                        .To<CombinedInputProvider>()
                        .AsSingle();
                    break;

                case InputProviderType.VirtualJoystick:
                    Container.Bind<VirtualJoystickInputProvider>().AsSingle();
                    Container.Bind<IInputProvider>()
                        .To<VirtualJoystickInputProvider>()
                        .AsSingle();
                    break;

                case InputProviderType.Keyboard:
                default:
                    Container.Bind<IInputProvider>()
                        .To<KeyboardInputProvider>()
                        .AsSingle();
                    break;
            }
        }

        private void InstallCommonServices()
        {
            Container.Bind<ScreenBounds>()
                .FromMethod(ctx => new ScreenBounds(Camera.main))
                .AsSingle();

            Container.Bind<EnemyFactory>().AsSingle();

            Container.Bind<ShipPresentation>().FromInstance(_shipPresentationPrefab).AsSingle();
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
            Container.BindInterfacesAndSelfTo<EnemySpawner>().FromInstance(_enemySpawner).AsSingle();
        }

        private void InstallParticleEffectSpawner()
        {
            Container.BindInterfacesAndSelfTo<ParticleEffectSpawner>().FromInstance(_particleEffectSpawner).AsSingle();
        }

        private void InstallLaserView()
        {
            Container.BindInterfacesAndSelfTo<LaserPresentation>().FromInstance(_laserPresentation).AsSingle();
        }

        private void InstallScoreUI()
        {
            Container.BindInterfacesAndSelfTo<ScoreViewModel>().AsSingle();
            Container.BindInterfacesAndSelfTo<ScoreView>().FromInstance(_scoreView).AsSingle();
        }

        private void InstallHealthUI()
        {
            Container.BindInterfacesAndSelfTo<HealthViewModel>().AsSingle();
            Container.BindInterfacesAndSelfTo<HealthView>().FromInstance(_healthView).AsSingle();
        }

        private void InstallPlayerStatsUI()
        {
            Container.BindInterfacesAndSelfTo<PlayerStatsViewModel>().AsSingle();
            Container.BindInterfacesAndSelfTo<PlayerStatsView>().FromInstance(_playerStatsView).AsSingle();
        }

        private void InstallGameController()
        {
            Container.BindInterfacesAndSelfTo<GameController>().AsSingle();
        }

        private void InstallStartMenuUI()
        {
            Container.BindInterfacesAndSelfTo<StartMenuViewModel>().AsSingle();
            Container.BindInterfacesAndSelfTo<StartMenuView>().FromInstance(_startMenuView).AsSingle();
        }

        private void InstallGameUI()
        {
            Container.BindInterfacesAndSelfTo<GameUIView>().FromInstance(_gameUIView).AsSingle();
        }
    }
}

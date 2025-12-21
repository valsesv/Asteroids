using UnityEngine;
using Zenject;
using Asteroids.Presentation.Enemies;
using Asteroids.Core.Entity.Components;
using UnityEngine.Assertions;

namespace Asteroids.Installers
{
    /// <summary>
    /// EnemyInstaller - installs bindings for enemy GameObjects
    /// Should be used with GameObjectContext on enemy GameObjects
    /// </summary>
    public class EnemyInstaller : MonoInstaller
    {
        [SerializeField] private EnemyView _enemyViewPrefab;

        public override void InstallBindings()
        {
            AssertEnemyViewPrefab();
            InstallSignals();
            InstallEnemyView();
        }

        private void AssertEnemyViewPrefab()
        {
            Assert.IsNotNull(_enemyViewPrefab, "EnemyViewPrefab is not assigned in EnemyInstaller!");
        }

        private void InstallSignals()
        {
            // Declare enemy-specific signals
            // These signals are separate from player signals declared in PlayerInstaller
            Container.DeclareSignal<TransformChangedSignal>();
            Container.DeclareSignal<PhysicsChangedSignal>();
        }

        private void InstallEnemyView()
        {
            Container.Bind<EnemyView>().FromInstance(_enemyViewPrefab).AsSingle();
            // EnemyView implements IInitializable and IDisposable for signal subscriptions
            Container.BindInterfacesTo<EnemyView>().FromInstance(_enemyViewPrefab);
        }
    }
}


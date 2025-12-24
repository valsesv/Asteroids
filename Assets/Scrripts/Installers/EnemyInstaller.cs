using UnityEngine;
using Zenject;
using Asteroids.Presentation.Enemies;
using Asteroids.Core.Entity.Components;
using UnityEngine.Assertions;

namespace Asteroids.Installers
{
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
            Container.DeclareSignal<TransformChangedSignal>();
            Container.DeclareSignal<PhysicsChangedSignal>();
        }

        private void InstallEnemyView()
        {
            Container.Bind<EnemyView>().FromInstance(_enemyViewPrefab).AsSingle();
            Container.BindInterfacesTo<EnemyView>().FromInstance(_enemyViewPrefab);
        }
    }
}


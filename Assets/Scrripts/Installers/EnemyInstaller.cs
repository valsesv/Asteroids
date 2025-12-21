using UnityEngine;
using Zenject;
using Asteroids.Presentation.Enemies;
using Asteroids.Core.Entity.Components;
using Asteroids.Core.Enemies;
using UnityEngine.Assertions;

namespace Asteroids.Installers
{
    public class EnemyInstaller : MonoInstaller
    {
        [SerializeField] private EnemyType _enemyType;
        [SerializeField] private EnemyView _enemyViewPrefab;

        public override void InstallBindings()
        {
            AssertEnemyViewPrefab();
            InstallSignals();
            InstallFactories();
            InstallEnemyView();
        }

        private void InstallFactories()
        {
            switch (_enemyType)
            {
                case EnemyType.Asteroid:
                    Container.Bind<AsteroidFactory>().AsSingle();
                    break;
                case EnemyType.Ufo:
                    Container.Bind<UfoFactory>().AsSingle();
                    break;
            }
        }

        private void AssertEnemyViewPrefab()
        {
            Assert.IsNotNull(_enemyViewPrefab, "EnemyViewPrefab is not assigned in EnemyInstaller!");
        }

        private void InstallSignals()
        {
            SignalBusInstaller.Install(Container);
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


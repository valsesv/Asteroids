using UnityEngine;
using Zenject;
using Asteroids.Presentation.Enemies;
using UnityEngine.Assertions;

namespace Asteroids.Installers
{
    public class EnemyInstaller : MonoInstaller
    {
        [SerializeField] private EnemyPresentation _enemyPresentationPrefab;

        public override void InstallBindings()
        {
            AssertEnemyViewPrefab();
            InstallSignals();
            InstallEnemyView();
        }

        private void AssertEnemyViewPrefab()
        {
            Assert.IsNotNull(_enemyPresentationPrefab, "EnemyPresentationPrefab is not assigned in EnemyInstaller!");
        }

        private void InstallSignals()
        {
        }

        private void InstallEnemyView()
        {
            Container.Bind<EnemyPresentation>().FromInstance(_enemyPresentationPrefab).AsSingle();
            Container.BindInterfacesTo<EnemyPresentation>().FromInstance(_enemyPresentationPrefab);
        }
    }
}


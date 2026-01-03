using UnityEngine;
using Zenject;
using Asteroids.Presentation.Player;
using UnityEngine.Assertions;

namespace Asteroids.Installers
{
    public class PlayerInstaller : MonoInstaller
    {
        [SerializeField] private ShipPresentation _shipPresentationPrefab;

        public override void InstallBindings()
        {
            AssertShipViewPrefab();
            InstallShipView();
        }

        private void AssertShipViewPrefab()
        {
            Assert.IsNotNull(_shipPresentationPrefab, "ShipPresentationPrefab is not assigned in PlayerInstaller!");
        }

        private void InstallShipView()
        {
            Container.Bind<ShipPresentation>().FromInstance(_shipPresentationPrefab).AsSingle();
            Container.BindInterfacesTo<ShipPresentation>().FromInstance(_shipPresentationPrefab);
        }
    }
}


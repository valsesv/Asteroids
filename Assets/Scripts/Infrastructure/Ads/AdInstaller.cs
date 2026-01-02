using Zenject;

namespace Asteroids.Infrastructure.Ads
{
    /// <summary>
    /// Installer for ads services
    /// Located in Assembly-CSharp to access YandexMobileAds
    /// Can be added to ProjectContext for global installation
    /// </summary>
    public class AdInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            // Bind AdSettings
            Container.Bind<AdSettings>().FromInstance(new AdSettings()).AsSingle();

            // Bind AdService
            Container.BindInterfacesAndSelfTo<AdService>().AsSingle();
        }
    }
}


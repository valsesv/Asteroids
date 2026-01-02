using Zenject;

namespace Asteroids.Infrastructure.Ads
{
    public class AdInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<AdSettings>().FromInstance(new AdSettings()).AsSingle();
            Container.BindInterfacesAndSelfTo<AdService>().AsSingle();
        }
    }
}


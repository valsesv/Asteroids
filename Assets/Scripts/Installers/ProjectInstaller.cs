using Zenject;
using Asteroids.Infrastructure.Firebase;
using Asteroids.Core.Game;

namespace Asteroids.Installers
{
    public class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            InstallSignalBus();
            InstallFirebase();
        }

        private void InstallSignalBus()
        {
            SignalBusInstaller.Install(Container);

            Container.DeclareSignal<GameStartedSignal>();
            Container.DeclareSignal<GameOverSignal>();
        }

        private void InstallFirebase()
        {
            Container.BindInterfacesAndSelfTo<FirebaseService>().AsSingle();
        }
    }
}

using Zenject;
using Asteroids.Infrastructure.Firebase;
using Asteroids.Core.Entity.Components;

namespace Asteroids.Installers
{
    /// <summary>
    /// ProjectInstaller - installs global bindings available in all scenes
    /// </summary>
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

            // Declare signals that are used in ProjectContext (Firebase, Ads, etc.)
            Container.DeclareSignal<GameStartedSignal>();
            Container.DeclareSignal<GameOverSignal>();
        }

        private void InstallFirebase()
        {
            Container.BindInterfacesAndSelfTo<FirebaseService>().AsSingle();
        }
    }
}

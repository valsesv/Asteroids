using Zenject;
using Asteroids.Infrastructure.Firebase;

namespace Asteroids.Installers
{
    /// <summary>
    /// ProjectInstaller - installs global bindings available in all scenes
    /// </summary>
    public class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            InstallFirebase();
        }

        private void InstallFirebase()
        {
            Container.BindInterfacesAndSelfTo<FirebaseService>().AsSingle();
        }
    }
}

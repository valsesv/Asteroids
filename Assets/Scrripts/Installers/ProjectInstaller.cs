using Zenject;

namespace Asteroids.Installers
{
    /// <summary>
    /// ProjectInstaller - installs global bindings available in all scenes
    /// SignalBus is automatically installed by Zenject through ZenjectManagersInstaller
    /// </summary>
    public class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            // SignalBus is already installed automatically through ZenjectManagersInstaller
            // Here you can add global bindings that are needed in all scenes
            // For example: game settings, analytics services, advertising services, etc.
        }
    }
}

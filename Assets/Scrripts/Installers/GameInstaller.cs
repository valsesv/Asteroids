using Zenject;

namespace Asteroids.Installers
{
    /// <summary>
    /// GameInstaller - installs bindings for the game scene
    /// SignalBus is available through ProjectContext
    /// </summary>
    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            // Here will be bindings for the game scene:
            // - Game logic (ship, asteroids, flying saucers)
            // - Factories for creating enemies
            // - Object Pool for asteroids and flying saucers
            // - Physics (custom)
            // - UI (MVVM)
            // - Facades
        }
    }
}

using Asteroids.Core.Entity;

namespace Asteroids.Core.Player
{
    /// <summary>
    /// Component that identifies an entity as a laser
    /// </summary>
    public class LaserComponent : IComponent
    {
        public float Duration { get; set; }
        public float Width { get; set; }

        public LaserComponent(float duration, float width)
        {
            Duration = duration;
            Width = width;
        }
    }
}


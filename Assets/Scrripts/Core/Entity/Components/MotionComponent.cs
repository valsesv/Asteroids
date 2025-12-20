namespace Asteroids.Core.Entity.Components
{
    /// <summary>
    /// Component for motion behavior (speed, acceleration settings)
    /// </summary>
    public class MotionComponent : IComponent
    {
        public float Speed { get; private set; }
        public float MaxSpeed { get; private set; }
        public float Acceleration { get; private set; }
        public float Friction { get; private set; }

        public MotionComponent(float maxSpeed = 5f, float acceleration = 10f, float friction = 0.98f)
        {
            MaxSpeed = maxSpeed;
            Acceleration = acceleration;
            Friction = friction;
            Speed = 0f;
        }
    }
}


using UnityEngine;
using Zenject;

namespace Asteroids.Core.Entity.Components
{
    /// <summary>
    /// Component for spatial transformation data (position, rotation)
    /// </summary>
    public class TransformComponent : IComponent
    {
        private readonly TransformChangedSignal _signal = new TransformChangedSignal();

        public Vector2 Position { get; private set; }
        public float Rotation { get; private set; }

        public TransformComponent(Vector2 position = default, float rotation = 0f)
        {
            Position = position;
            Rotation = rotation;
        }

        public void SetPosition(Vector2 position, SignalBus signalBus = null)
        {
            Position = position;
            FireSignal(signalBus);
        }

        public void SetRotation(float rotation, SignalBus signalBus = null)
        {
            Rotation = rotation;
            FireSignal(signalBus);
        }

        public void Move(Vector2 delta, SignalBus signalBus = null)
        {
            Position += delta;
            FireSignal(signalBus);
        }

        private void FireSignal(SignalBus signalBus)
        {
            if (signalBus != null)
            {
                _signal.X = Position.x;
                _signal.Y = Position.y;
                _signal.Rotation = Rotation;
                signalBus.Fire(_signal);
            }
        }
    }
}


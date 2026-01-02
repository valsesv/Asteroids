using UnityEngine;
using Zenject;

namespace Asteroids.Core.Entity.Components
{
    public class TransformComponent : IComponent
    {
        private readonly TransformChangedSignal _signal = new TransformChangedSignal();
        private readonly SignalBus _signalBus;

        public Vector2 Position { get; private set; }
        public float Rotation { get; private set; }

        public TransformComponent(Vector2 position = default, float rotation = 0f, SignalBus signalBus = null)
        {
            Position = position;
            Rotation = rotation;
            _signalBus = signalBus;
        }

        public void SetPosition(Vector2 position)
        {
            Position = position;
            FireSignal();
        }

        public void SetRotation(float rotation)
        {
            Rotation = rotation;
            FireSignal();
        }

        public void Move(Vector2 delta)
        {
            Position += delta;
            FireSignal();
        }

        private void FireSignal()
        {
            if (_signalBus == null)
            {
                return;
            }
            _signal.X = Position.x;
            _signal.Y = Position.y;
            _signal.Rotation = Rotation;
            _signalBus.Fire(_signal);
        }
    }
}


using Zenject;

namespace Asteroids.Core.Entity.Components
{
    /// <summary>
    /// Component that wraps objects around screen boundaries
    /// When object reaches one edge, it appears on the opposite edge
    /// </summary>
    public class ScreenWrapComponent : ITickableComponent
    {
        private readonly TransformComponent _transform;
        private readonly ScreenBounds _screenBounds;
        private readonly SignalBus _signalBus;

        public ScreenWrapComponent(TransformComponent transform, ScreenBounds screenBounds, SignalBus signalBus)
        {
            _transform = transform;
            _screenBounds = screenBounds;
            _signalBus = signalBus;
        }

        public void Tick()
        {
            var position = _transform.Position;
            var newPosition = position;
            bool positionChanged = false;

            // Wrap horizontally
            if (position.x > _screenBounds.Right)
            {
                newPosition.x = _screenBounds.Left;
                positionChanged = true;
            }
            else if (position.x < _screenBounds.Left)
            {
                newPosition.x = _screenBounds.Right;
                positionChanged = true;
            }

            // Wrap vertically
            if (position.y > _screenBounds.Top)
            {
                newPosition.y = _screenBounds.Bottom;
                positionChanged = true;
            }
            else if (position.y < _screenBounds.Bottom)
            {
                newPosition.y = _screenBounds.Top;
                positionChanged = true;
            }

            // Update position if it was wrapped
            if (positionChanged)
            {
                _transform.SetPosition(newPosition);
            }
        }
    }
}


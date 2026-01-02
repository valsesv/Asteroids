using UnityEngine;
using Zenject;

namespace Asteroids.Core.Entity.Components
{
    /// <summary>
    /// Component that wraps objects around screen boundaries
    /// When object reaches one edge, it appears on the opposite edge
    /// Only wraps objects that have entered the game area (prevents immediate teleportation on spawn)
    /// </summary>
    public class ScreenWrapComponent : ITickableComponent
    {
        private readonly TransformComponent _transform;
        private readonly ScreenBounds _screenBounds;
        private readonly SignalBus _signalBus;
        private bool _isInGameArea = false;

        public ScreenWrapComponent(TransformComponent transform, ScreenBounds screenBounds, SignalBus signalBus)
        {
            _transform = transform;
            _screenBounds = screenBounds;
            _signalBus = signalBus;
        }

        public void Tick()
        {
            var position = _transform.Position;

            UpdateGameAreaFlag(position);

            if (!_isInGameArea)
            {
                return;
            }

            WrapPosition(position);
        }

        private void UpdateGameAreaFlag(Vector2 position)
        {
            bool isCurrentlyInGameArea = position.x >= _screenBounds.Left &&
                   position.x <= _screenBounds.Right &&
                   position.y >= _screenBounds.Bottom &&
                   position.y <= _screenBounds.Top;

            if (isCurrentlyInGameArea && !_isInGameArea)
            {
                _isInGameArea = true;
            }
        }

        private void WrapPosition(Vector2 position)
        {
            Vector2 newPosition = position;
            bool positionChanged = false;

            positionChanged |= WrapHorizontally(ref newPosition, position);
            positionChanged |= WrapVertically(ref newPosition, position);

            if (positionChanged)
            {
                _transform.SetPosition(newPosition);
            }
        }

        private bool WrapHorizontally(ref Vector2 newPosition, Vector2 position)
        {
            if (position.x > _screenBounds.Right)
            {
                newPosition.x = _screenBounds.Left;
                return true;
            }
            else if (position.x < _screenBounds.Left)
            {
                newPosition.x = _screenBounds.Right;
                return true;
            }

            return false;
        }

        private bool WrapVertically(ref Vector2 newPosition, Vector2 position)
        {
            if (position.y > _screenBounds.Top)
            {
                newPosition.y = _screenBounds.Bottom;
                return true;
            }
            else if (position.y < _screenBounds.Bottom)
            {
                newPosition.y = _screenBounds.Top;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Reset the game area flag - call this when reusing object from pool
        /// </summary>
        public void Reset()
        {
            _isInGameArea = false;
        }
    }
}


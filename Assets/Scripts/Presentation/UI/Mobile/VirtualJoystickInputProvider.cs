using UnityEngine;
using Asteroids.Core.PlayerInput;

namespace Asteroids.Presentation.UI
{
    public class VirtualJoystickInputProvider : IInputProvider
    {
        private readonly VirtualJoystickView _joystickView;
        private readonly MobileInputView _mobileInputView;
        private bool _wasLaserPressedLastFrame = false;

        public VirtualJoystickInputProvider(VirtualJoystickView joystickView, MobileInputView mobileInputView)
        {
            _joystickView = joystickView;
            _mobileInputView = mobileInputView;
        }

        public float GetForwardInput()
        {
            if (_joystickView == null)
            {
                return 0f;
            }

            return _joystickView.Direction.y;
        }

        public float GetRotationInput()
        {
            return 0f;
        }

        public Vector2 GetDirectionInput()
        {
            if (_joystickView == null)
            {
                return Vector2.zero;
            }

            return _joystickView.Direction;
        }

        public bool GetShootBulletInput()
        {
            if (_mobileInputView == null)
            {
                return false;
            }

            return _mobileInputView.IsShootBulletPressed;
        }

        public bool GetShootLaserInput()
        {
            if (_mobileInputView == null)
            {
                _wasLaserPressedLastFrame = false;
                return false;
            }

            bool isPressed = _mobileInputView.IsShootLaserPressed;
            bool justPressed = isPressed && !_wasLaserPressedLastFrame;
            _wasLaserPressedLastFrame = isPressed;

            return justPressed;
        }
    }
}
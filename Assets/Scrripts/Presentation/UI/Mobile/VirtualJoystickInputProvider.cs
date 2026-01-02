using UnityEngine;
using Asteroids.Core.PlayerInput;

namespace Asteroids.Presentation.UI
{
    /// <summary>
    /// Virtual joystick input provider for mobile devices
    /// Reads input from VirtualJoystickView and MobileInputView
    /// </summary>
    public class VirtualJoystickInputProvider : IInputProvider
    {
        private readonly VirtualJoystickView _joystickView;
        private readonly MobileInputView _mobileInputView;

        public VirtualJoystickInputProvider(VirtualJoystickView joystickView, MobileInputView mobileInputView)
        {
            _joystickView = joystickView;
            _mobileInputView = mobileInputView;
        }

        public float GetForwardInput()
        {
            if (_joystickView == null)
                return 0f;

            // Joystick Y axis: positive = forward, negative = backward
            return _joystickView.Direction.y;
        }

        public float GetRotationInput()
        {
            if (_joystickView == null)
                return 0f;

            // Joystick X axis: positive = right rotation, negative = left rotation
            return _joystickView.Direction.x;
        }

        public bool GetShootBulletInput()
        {
            if (_mobileInputView == null)
                return false;

            return _mobileInputView.IsShootBulletPressed;
        }

        public bool GetShootLaserInput()
        {
            if (_mobileInputView == null)
                return false;

            return _mobileInputView.IsShootLaserPressed;
        }
    }
}


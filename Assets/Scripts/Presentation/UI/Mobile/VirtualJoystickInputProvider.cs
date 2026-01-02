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
        private bool _wasLaserPressedLastFrame = false;

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
            // For mobile, rotation is handled via direction input
            // Return 0 to use direction-based movement instead
            return 0f;
        }

        public Vector2 GetDirectionInput()
        {
            if (_joystickView == null)
                return Vector2.zero;

            // Return joystick direction directly
            // Y is forward/backward, X is left/right
            return _joystickView.Direction;
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
            {
                _wasLaserPressedLastFrame = false;
                return false;
            }

            // Check if button is currently pressed
            bool isPressed = _mobileInputView.IsShootLaserPressed;

            // Return true only when button state changes from false to true (just pressed)
            bool justPressed = isPressed && !_wasLaserPressedLastFrame;

            // Update previous state for next frame
            _wasLaserPressedLastFrame = isPressed;

            return justPressed;
        }
    }
}


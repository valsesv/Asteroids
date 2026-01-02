using UnityEngine;
using Asteroids.Core.PlayerInput;

namespace Asteroids.Presentation.UI
{
    /// <summary>
    /// Combined input provider that uses both keyboard/mouse and mobile input
    /// Useful for Editor where both input methods should be available
    /// </summary>
    public class CombinedInputProvider : IInputProvider
    {
        private readonly KeyboardInputProvider _keyboardProvider;
        private readonly VirtualJoystickInputProvider _mobileProvider;

        public CombinedInputProvider(
            KeyboardInputProvider keyboardProvider,
            VirtualJoystickInputProvider mobileProvider)
        {
            _keyboardProvider = keyboardProvider;
            _mobileProvider = mobileProvider;
        }

        public float GetForwardInput()
        {
            // Use keyboard input if available, otherwise use mobile
            float keyboardInput = _keyboardProvider?.GetForwardInput() ?? 0f;
            float mobileInput = _mobileProvider?.GetForwardInput() ?? 0f;

            // Combine inputs (take the one with larger magnitude)
            return Mathf.Abs(keyboardInput) > Mathf.Abs(mobileInput) ? keyboardInput : mobileInput;
        }

        public float GetRotationInput()
        {
            // Use keyboard input if available, otherwise use mobile
            float keyboardInput = _keyboardProvider?.GetRotationInput() ?? 0f;
            float mobileInput = _mobileProvider?.GetRotationInput() ?? 0f;

            // Combine inputs (take the one with larger magnitude)
            return Mathf.Abs(keyboardInput) > Mathf.Abs(mobileInput) ? keyboardInput : mobileInput;
        }

        public bool GetShootBulletInput()
        {
            // Check both inputs - if either is pressed, return true
            bool keyboardInput = _keyboardProvider?.GetShootBulletInput() ?? false;
            bool mobileInput = _mobileProvider?.GetShootBulletInput() ?? false;

            return keyboardInput || mobileInput;
        }

        public bool GetShootLaserInput()
        {
            // Check both inputs - if either is pressed, return true
            bool keyboardInput = _keyboardProvider?.GetShootLaserInput() ?? false;
            bool mobileInput = _mobileProvider?.GetShootLaserInput() ?? false;

            return keyboardInput || mobileInput;
        }
    }
}


using UnityEngine;
using Asteroids.Core.PlayerInput;

namespace Asteroids.Presentation.UI
{
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
            float keyboardInput = _keyboardProvider?.GetForwardInput() ?? 0f;
            float mobileInput = _mobileProvider?.GetForwardInput() ?? 0f;

            return Mathf.Abs(keyboardInput) > Mathf.Abs(mobileInput) ? keyboardInput : mobileInput;
        }

        public float GetRotationInput()
        {
            float keyboardInput = _keyboardProvider?.GetRotationInput() ?? 0f;
            float mobileInput = _mobileProvider?.GetRotationInput() ?? 0f;

            return Mathf.Abs(keyboardInput) > Mathf.Abs(mobileInput) ? keyboardInput : mobileInput;
        }

        public Vector2 GetDirectionInput()
        {
            Vector2 mobileInput = _mobileProvider?.GetDirectionInput() ?? Vector2.zero;

            if (mobileInput.magnitude > 0.01f)
            {
                return mobileInput;
            }

            return Vector2.zero;
        }

        public bool GetShootBulletInput()
        {
            bool keyboardInput = _keyboardProvider?.GetShootBulletInput() ?? false;
            bool mobileInput = _mobileProvider?.GetShootBulletInput() ?? false;

            return keyboardInput || mobileInput;
        }

        public bool GetShootLaserInput()
        {
            bool keyboardInput = _keyboardProvider?.GetShootLaserInput() ?? false;
            bool mobileInput = _mobileProvider?.GetShootLaserInput() ?? false;

            return keyboardInput || mobileInput;
        }
    }
}
using UnityEngine;

namespace Asteroids.Core.PlayerInput
{
    /// <summary>
    /// Keyboard input provider for ship movement
    /// </summary>
    public class KeyboardInputProvider : IInputProvider
    {
        private readonly KeyboardInputSettings _settings;

        public KeyboardInputProvider(KeyboardInputSettings settings)
        {
            _settings = settings;
        }

        public float GetForwardInput()
        {
            float input = 0f;

            if (Input.GetKey(_settings.MoveUp))
                input += 1f;
            if (Input.GetKey(_settings.MoveDown))
                input -= 1f;

            return Mathf.Clamp(input, -1f, 1f);
        }

        public float GetRotationInput()
        {
            float input = 0f;

            if (Input.GetKey(_settings.MoveRight))
                input += 1f;
            if (Input.GetKey(_settings.MoveLeft))
                input -= 1f;

            return Mathf.Clamp(input, -1f, 1f);
        }

        public bool GetShootBulletInput()
        {
            return Input.GetKey(KeyCode.Space);
        }

        public bool GetShootLaserInput()
        {
            return Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift);
        }
    }
}


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

        public Vector2 GetMovementInput()
        {
            Vector2 input = Vector2.zero;

            if (Input.GetKey(_settings.MoveUp))
                input.y += 1f;
            if (Input.GetKey(_settings.MoveDown))
                input.y -= 1f;
            if (Input.GetKey(_settings.MoveLeft))
                input.x -= 1f;
            if (Input.GetKey(_settings.MoveRight))
                input.x += 1f;

            // Normalize to prevent faster diagonal movement
            return input.normalized;
        }
    }
}


using UnityEngine;

namespace Asteroids.Core.PlayerInput
{
    /// <summary>
    /// Keyboard and mouse input provider for ship movement and shooting
    /// Supports multiple keys for each action and mouse buttons
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

            // Check all forward keys
            if (IsAnyKeyPressed(_settings.MoveUpKeys))
                input += 1f;

            // Check all backward keys
            if (IsAnyKeyPressed(_settings.MoveDownKeys))
                input -= 1f;

            return Mathf.Clamp(input, -1f, 1f);
        }

        public float GetRotationInput()
        {
            float input = 0f;

            // Check all right rotation keys
            if (IsAnyKeyPressed(_settings.MoveRightKeys))
                input += 1f;

            // Check all left rotation keys
            if (IsAnyKeyPressed(_settings.MoveLeftKeys))
                input -= 1f;

            return Mathf.Clamp(input, -1f, 1f);
        }

        public bool GetShootBulletInput()
        {
            // Check keyboard keys
            if (IsAnyKeyPressed(_settings.ShootBulletKeys))
                return true;

            // Check mouse buttons
            if (IsAnyMouseButtonPressed(_settings.ShootBulletMouseButtons))
                return true;

            return false;
        }

        public bool GetShootLaserInput()
        {
            // Check keyboard keys (use GetKeyDown for single shot)
            if (IsAnyKeyDown(_settings.ShootLaserKeys))
                return true;

            // Check mouse buttons (use GetMouseButtonDown for single shot)
            if (IsAnyMouseButtonDown(_settings.ShootLaserMouseButtons))
                return true;

            return false;
        }

        /// <summary>
        /// Check if any of the specified keys is currently pressed
        /// </summary>
        private bool IsAnyKeyPressed(KeyCode[] keys)
        {
            if (keys == null || keys.Length == 0)
                return false;

            foreach (var key in keys)
            {
                if (Input.GetKey(key))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Check if any of the specified keys was pressed this frame
        /// </summary>
        private bool IsAnyKeyDown(KeyCode[] keys)
        {
            if (keys == null || keys.Length == 0)
                return false;

            foreach (var key in keys)
            {
                if (Input.GetKeyDown(key))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Check if any of the specified mouse buttons is currently pressed
        /// </summary>
        private bool IsAnyMouseButtonPressed(int[] mouseButtons)
        {
            if (mouseButtons == null || mouseButtons.Length == 0)
                return false;

            foreach (var button in mouseButtons)
            {
                if (button >= 0 && button <= 2 && Input.GetMouseButton(button))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Check if any of the specified mouse buttons was pressed this frame
        /// </summary>
        private bool IsAnyMouseButtonDown(int[] mouseButtons)
        {
            if (mouseButtons == null || mouseButtons.Length == 0)
                return false;

            foreach (var button in mouseButtons)
            {
                if (button >= 0 && button <= 2 && Input.GetMouseButtonDown(button))
                    return true;
            }

            return false;
        }
    }
}


using UnityEngine;
using UnityEngine.EventSystems;

namespace Asteroids.Core.PlayerInput
{
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

            if (IsAnyKeyPressed(_settings.MoveUpKeys))
                input += 1f;

            if (IsAnyKeyPressed(_settings.MoveDownKeys))
                input -= 1f;

            return Mathf.Clamp(input, -1f, 1f);
        }

        public float GetRotationInput()
        {
            float input = 0f;

            if (IsAnyKeyPressed(_settings.MoveRightKeys))
                input += 1f;

            if (IsAnyKeyPressed(_settings.MoveLeftKeys))
                input -= 1f;

            return Mathf.Clamp(input, -1f, 1f);
        }

        public Vector2 GetDirectionInput()
        {
            return Vector2.zero;
        }

        public bool GetShootBulletInput()
        {
            if (IsAnyKeyPressed(_settings.ShootBulletKeys))
                return true;

            if (!IsPointerOverUI() && IsAnyMouseButtonPressed(_settings.ShootBulletMouseButtons))
                return true;

            return false;
        }

        public bool GetShootLaserInput()
        {
            if (IsAnyKeyDown(_settings.ShootLaserKeys))
                return true;

            if (!IsPointerOverUI() && IsAnyMouseButtonDown(_settings.ShootLaserMouseButtons))
                return true;

            return false;
        }

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

        private bool IsPointerOverUI()
        {
            if (EventSystem.current != null)
            {
                return EventSystem.current.IsPointerOverGameObject();
            }

            return false;
        }
    }
}


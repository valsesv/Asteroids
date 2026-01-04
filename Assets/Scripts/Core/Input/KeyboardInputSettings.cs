using UnityEngine;

namespace Asteroids.Core.PlayerInput
{
    [CreateAssetMenu(fileName = "KeyboardInputSettings", menuName = "Game/Settings/Keyboard Input Settings")]
    public class KeyboardInputSettings : ScriptableObject
    {
        [Header("Movement Forward/Backward")]
        [Tooltip("Keys for moving forward (WASD and Arrow keys)")]
        public KeyCode[] MoveUpKeys = { KeyCode.W, KeyCode.UpArrow };

        [Tooltip("Keys for moving backward")]
        public KeyCode[] MoveDownKeys = { KeyCode.S, KeyCode.DownArrow };

        [Header("Rotation Left/Right")]
        [Tooltip("Keys for rotating left")]
        public KeyCode[] MoveLeftKeys = { KeyCode.A, KeyCode.LeftArrow };

        [Tooltip("Keys for rotating right")]
        public KeyCode[] MoveRightKeys = { KeyCode.D, KeyCode.RightArrow };

        [Header("Shooting - Keyboard")]
        [Tooltip("Keys for shooting bullets")]
        public KeyCode[] ShootBulletKeys = { KeyCode.Space };

        [Tooltip("Keys for shooting laser")]
        public KeyCode[] ShootLaserKeys = { KeyCode.LeftShift, KeyCode.RightShift };

        [Header("Shooting - Mouse")]
        [Tooltip("Mouse buttons for shooting bullets (0=Left, 1=Right, 2=Middle). Empty array disables mouse for bullets.")]
        public int[] ShootBulletMouseButtons = { 0 };

        [Tooltip("Mouse buttons for shooting laser (0=Left, 1=Right, 2=Middle). Empty array disables mouse for laser.")]
        public int[] ShootLaserMouseButtons = { 1 };
    }
}
using UnityEngine;

namespace Asteroids.Core.PlayerInput
{
    [CreateAssetMenu(fileName = "KeyboardInputSettings", menuName = "Game/Settings/Keyboard Input Settings")]
    public class KeyboardInputSettings : ScriptableObject
    {
        public KeyCode MoveUp = KeyCode.W;
        public KeyCode MoveDown = KeyCode.S;
        public KeyCode MoveLeft = KeyCode.A;
        public KeyCode MoveRight = KeyCode.D;
    }
}


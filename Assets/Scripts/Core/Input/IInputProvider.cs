using UnityEngine;

namespace Asteroids.Core.PlayerInput
{
    /// <summary>
    /// Interface for input providers
    /// </summary>
    public interface IInputProvider
    {
        /// <summary>
        /// Get forward/backward movement input (W/S keys)
        /// Returns: positive for forward, negative for backward
        /// </summary>
        float GetForwardInput();
        
        /// <summary>
        /// Get rotation input (A/D keys)
        /// Returns: positive for right rotation, negative for left rotation
        /// </summary>
        float GetRotationInput();
        
        /// <summary>
        /// Get direction input as vector (for joystick/touch)
        /// Returns: normalized direction vector, or zero if not available
        /// </summary>
        Vector2 GetDirectionInput();
        
        bool GetShootBulletInput();
        
        /// <summary>
        /// Get laser shoot input
        /// Returns: true when player wants to shoot laser
        /// </summary>
        bool GetShootLaserInput();
    }
}


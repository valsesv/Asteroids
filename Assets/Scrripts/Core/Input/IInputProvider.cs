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
        
        bool GetShootBulletInput();
    }
}


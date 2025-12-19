using UnityEngine;

namespace Asteroids.Core.PlayerInput
{
    /// <summary>
    /// Interface for input providers
    /// </summary>
    public interface IInputProvider
    {
        Vector2 GetMovementInput();
    }
}


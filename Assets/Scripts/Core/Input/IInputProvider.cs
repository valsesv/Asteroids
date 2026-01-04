using UnityEngine;

namespace Asteroids.Core.PlayerInput
{
    public interface IInputProvider
    {
        float GetForwardInput();

        float GetRotationInput();

        Vector2 GetDirectionInput();

        bool GetShootBulletInput();

        bool GetShootLaserInput();
    }
}
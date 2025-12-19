using UnityEngine;
using Zenject;

namespace Asteroids.Core.Player
{
    /// <summary>
    /// Initializes ship model with default values
    /// </summary>
    public class ShipInitializer : IInitializable
    {
        private readonly ShipModel _shipModel;

        public ShipInitializer(ShipModel shipModel)
        {
            _shipModel = shipModel;
        }

        public void Initialize()
        {
            // Initialize ship position at center
            _shipModel.Position = Vector2.zero;
            _shipModel.Rotation = 0f;
            _shipModel.Velocity = Vector2.zero;

            // Initialize state
            _shipModel.CanControl = true;
        }
    }
}


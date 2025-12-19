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
        private readonly StartPositionSettings _startPositionSettings;

        public ShipInitializer(ShipModel shipModel, StartPositionSettings startPositionSettings)
        {
            _shipModel = shipModel;
            _startPositionSettings = startPositionSettings;
        }

        public void Initialize()
        {
            _shipModel.Position = _startPositionSettings.Position;
            _shipModel.Rotation = _startPositionSettings.Rotation;
            _shipModel.Velocity = Vector2.zero;

            // Initialize state
            _shipModel.CanControl = true;
        }
    }
}


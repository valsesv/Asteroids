using System;
using UnityEngine;
using Zenject;
using Asteroids.Core.Signals;

namespace Asteroids.Presentation.Player
{
    /// <summary>
    /// Ship view - MonoBehaviour that represents the ship in the scene
    /// Subscribes to signals directly (no ViewModel needed for game objects)
    /// </summary>
    public class ShipView : MonoBehaviour, IInitializable, IDisposable
    {
        [Inject] private SignalBus _signalBus;

        public void Initialize()
        {
            _signalBus.Subscribe<ShipPositionChangedSignal>(OnPositionChanged);
            _signalBus.Subscribe<ShipVelocityChangedSignal>(OnVelocityChanged);
        }

        public void Dispose()
        {
            _signalBus?.Unsubscribe<ShipPositionChangedSignal>(OnPositionChanged);
            _signalBus?.Unsubscribe<ShipVelocityChangedSignal>(OnVelocityChanged);
        }

        private void OnPositionChanged(ShipPositionChangedSignal signal)
        {
            transform.position = new Vector3(signal.X, signal.Y, 0f);
            transform.rotation = Quaternion.Euler(0f, 0f, signal.Rotation);
        }

        private void OnVelocityChanged(ShipVelocityChangedSignal signal)
        {
            // Can be used for visual effects based on speed
        }
    }
}


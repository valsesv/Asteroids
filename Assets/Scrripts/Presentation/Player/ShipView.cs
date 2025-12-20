using System;
using UnityEngine;
using Zenject;
using Asteroids.Core.Entity.Components;

namespace Asteroids.Presentation.Player
{
    /// <summary>
    /// Ship view - MonoBehaviour that represents the ship in the scene
    /// Subscribes to component signals directly (no ViewModel needed for game objects)
    /// </summary>
    public class ShipView : MonoBehaviour, IInitializable, IDisposable
    {
        [Inject] private SignalBus _signalBus;

        public void Initialize()
        {
            _signalBus.Subscribe<TransformChangedSignal>(OnTransformChanged);
            _signalBus.Subscribe<PhysicsChangedSignal>(OnPhysicsChanged);
        }

        public void Dispose()
        {
            _signalBus?.Unsubscribe<TransformChangedSignal>(OnTransformChanged);
            _signalBus?.Unsubscribe<PhysicsChangedSignal>(OnPhysicsChanged);
        }

        private void OnTransformChanged(TransformChangedSignal signal)
        {
            transform.position = new Vector3(signal.X, signal.Y, 0f);
            transform.rotation = Quaternion.Euler(0f, 0f, signal.Rotation);
        }

        private void OnPhysicsChanged(PhysicsChangedSignal signal)
        {
            // Can be used for visual effects based on speed
        }
    }
}


using System;
using UnityEngine;
using Zenject;
using Asteroids.Core.Entity.Components;
using Asteroids.Core.Player;

namespace Asteroids.Presentation.Player
{
    /// <summary>
    /// Ship view - MonoBehaviour that represents the ship in the scene
    /// Subscribes to component signals directly (no ViewModel needed for game objects)
    /// </summary>
    public class ShipView : MonoBehaviour, IInitializable, IDisposable
    {
        public ShipModel Entity { get; private set; }

        [Inject] private SignalBus _signalBus;

        [Inject]
        public void Construct(ShipModel shipModel)
        {
            Entity = shipModel;
        }

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

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Debug.Log($"[ShipView] Collision entered with: {collision.gameObject.name}");
        }
    }
}


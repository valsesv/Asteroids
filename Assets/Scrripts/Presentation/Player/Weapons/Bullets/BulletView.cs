using System;
using UnityEngine;
using Zenject;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;
using Asteroids.Core.Player;

namespace Asteroids.Presentation.Player
{
    /// <summary>
    /// Bullet view - MonoBehaviour that represents a bullet in the scene
    /// </summary>
    public class BulletView : MonoBehaviour, IInitializable, IDisposable
    {
        public GameEntity Entity { get; private set; }

        [Inject] private SignalBus _signalBus;

        [Inject]
        public void Construct(GameEntity entity)
        {
            Entity = entity;
        }

        public void Initialize()
        {
            _signalBus.Subscribe<TransformChangedSignal>(OnTransformChanged);
        }

        public void Dispose()
        {
            _signalBus?.Unsubscribe<TransformChangedSignal>(OnTransformChanged);
        }

        private void OnTransformChanged(TransformChangedSignal signal)
        {
            transform.position = new Vector3(signal.X, signal.Y, 0f);
            transform.rotation = Quaternion.Euler(0f, 0f, signal.Rotation);
        }
    }
}


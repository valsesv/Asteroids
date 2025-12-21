using System;
using UnityEngine;
using Zenject;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;

namespace Asteroids.Presentation.Enemies
{
    /// <summary>
    /// Base enemy view - MonoBehaviour that represents enemy in the scene
    /// Subscribes to component signals for position/rotation updates
    /// </summary>
    public abstract class EnemyView : MonoBehaviour, IInitializable, IDisposable
    {
        protected GameEntity Entity;

        [Inject] protected SignalBus _signalBus;

        public virtual void Initialize()
        {
            _signalBus.Subscribe<TransformChangedSignal>(OnTransformChanged);
        }

        public void Dispose()
        {
            _signalBus?.Unsubscribe<TransformChangedSignal>(OnTransformChanged);
        }

        protected virtual void OnTransformChanged(TransformChangedSignal signal)
        {
            // Update Unity transform from signal
            transform.position = new Vector3(signal.X, signal.Y, 0f);
            transform.rotation = Quaternion.Euler(0f, 0f, signal.Rotation);
        }
    }
}


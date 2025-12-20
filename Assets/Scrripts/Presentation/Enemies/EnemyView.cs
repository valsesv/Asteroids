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

        [Inject] private SignalBus _signalBus;

        public void Initialize(GameEntity entity)
        {
            Entity = entity;
        }

        public void Initialize()
        {
            // Subscribe to transform changes
            _signalBus.Subscribe<TransformChangedSignal>(OnTransformChanged);
        }

        public void Dispose()
        {
            _signalBus?.Unsubscribe<TransformChangedSignal>(OnTransformChanged);
        }

        private void OnTransformChanged(TransformChangedSignal signal)
        {
        }
    }
}


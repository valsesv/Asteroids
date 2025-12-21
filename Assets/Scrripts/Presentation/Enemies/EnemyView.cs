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

        /// <summary>
        /// Set spawn position - updates both Unity transform and TransformComponent
        /// Call this when spawning or reusing enemy from pool
        /// </summary>
        public void SetSpawnPosition(Vector2 position)
        {
            // Update Unity transform
            transform.position = new Vector3(position.x, position.y, 0f);

            // Update TransformComponent if Entity is already created
            if (Entity == null)
            {
                return;
            }

            var transformComponent = Entity.GetComponent<TransformComponent>();
            if (transformComponent != null)
            {
                transformComponent.SetPosition(position);
            }

            // Reset ScreenWrapComponent flag when reusing from pool
            var screenWrap = Entity.GetComponent<ScreenWrapComponent>();
            if (screenWrap != null)
            {
                screenWrap.Reset();
            }
        }

        protected virtual void OnCollisionEnter2D(Collision2D collision)
        {
            Debug.Log($"[EnemyView] {gameObject.name} collision entered with: {collision.gameObject.name}");
        }
    }
}


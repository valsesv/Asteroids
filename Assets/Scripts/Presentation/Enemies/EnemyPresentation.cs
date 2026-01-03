using UnityEngine;
using Zenject;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;
using Asteroids.Presentation.Player;
using Asteroids.Presentation.Effects;
using Asteroids.Core.Player;

namespace Asteroids.Presentation.Enemies
{
    public abstract class EnemyPresentation : MonoBehaviour, IInitializable
    {
        public GameEntity Entity { get; protected set; }

        [Inject] protected EnemySpawner _enemySpawner;
        [Inject] protected TickableManager _tickableManager;
        [Inject] protected DiContainer _container;
        [Inject] protected ParticleEffectSpawner _particleEffectSpawner;

        protected TransformComponent _transformComponent;
        private ScreenWrapComponent _screenWrapComponent;

        public virtual void Initialize()
        {
            _transformComponent = Entity?.GetComponent<TransformComponent>();
            _screenWrapComponent = Entity?.GetComponent<ScreenWrapComponent>();
        }

        private void LateUpdate()
        {
            if (_transformComponent != null)
            {
                transform.position = new Vector3(_transformComponent.Position.x, _transformComponent.Position.y, 0f);
                transform.rotation = Quaternion.Euler(0f, 0f, _transformComponent.Rotation);
            }
        }

        public void SetSpawnPosition(Vector2 position)
        {
            transform.position = new Vector3(position.x, position.y, 0f);

            if (Entity == null)
            {
                return;
            }

            var transformComponent = Entity.GetComponent<TransformComponent>();
            transformComponent.SetPosition(position);

            var screenWrap = Entity.GetComponent<ScreenWrapComponent>();
            screenWrap.Reset();
        }

        public virtual void GetDamage()
        {
            HandleDeath();
        }

        public virtual void HandleDeath()
        {
            if (_screenWrapComponent.IsInGameArea == false)
            {
                return;
            }

            _particleEffectSpawner.SpawnExplosion(transform.position);
            _enemySpawner.ReturnEnemy(this);
        }

        protected virtual void OnCollisionEnter2D(Collision2D collision)
        {
            var bulletPresentation = collision.gameObject.GetComponent<BulletPresentation>();
            if (bulletPresentation != null)
            {
                GetDamage();
                return;
            }

            if (collision.gameObject.TryGetComponent<ShipPresentation>(out var shipPresentation))
            {
                HandleShipCollision(shipPresentation);
            }
        }

        private void HandleShipCollision(ShipPresentation shipPresentation)
        {
            if (shipPresentation.Entity.GetComponent<DamageHandler>().IsInvincible != false)
            {
                return;
            }
            GetDamage();
        }
    }
}


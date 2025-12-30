using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;
using UnityEngine.Assertions;

namespace Asteroids.Presentation.Player
{
    /// <summary>
    /// Manager that handles creation and destruction of bullet views
    /// </summary>
    public class ProjectileSpawner : MonoBehaviour, IInitializable, IDisposable
    {
        [SerializeField] private GameObject _bulletPrefab;
        [SerializeField] private Transform _bulletParent;

        private SignalBus _signalBus;
        private DiContainer _container;
        private TickableManager _tickableManager;

        // Object pools for projectiles
        private ObjectPool<BulletView> _bulletPool;

        // Factories for creating projectiles (like enemies)
        private ProjectileViewFactory<BulletView> _bulletFactory;

        private List<BulletView> _activeBullets = new List<BulletView>();

        [Inject]
        public void Construct(SignalBus signalBus, DiContainer container, TickableManager tickableManager)
        {
            _signalBus = signalBus;
            _container = container;
            _tickableManager = tickableManager;
        }

        public void Initialize()
        {
            Assert.IsNotNull(_bulletPrefab, "BulletPrefab is not assigned in ProjectileSpawner!");
            Assert.IsNotNull(_bulletParent, "BulletParent is not assigned in ProjectileSpawner!");

            _bulletFactory = new ProjectileViewFactory<BulletView>(_container, _bulletPrefab, _bulletParent);
            _bulletPool = new ObjectPool<BulletView>(() => _bulletFactory.Create(Vector2.zero), _bulletParent);

            // Subscribe to signals
            _signalBus.Subscribe<BulletCreatedSignal>(OnBulletCreated);
            _signalBus.Subscribe<BulletDestroyedSignal>(OnBulletDestroyed);
        }

        public void Dispose()
        {
            _signalBus?.Unsubscribe<BulletCreatedSignal>(OnBulletCreated);
            _signalBus?.Unsubscribe<BulletDestroyedSignal>(OnBulletDestroyed);
        }

        private void OnBulletCreated(BulletCreatedSignal signal)
        {
            var spawnPosition = signal.Entity.GetComponent<TransformComponent>().Position;

            // Create subcontainer for this bullet to track its signals
            var subContainer = _container.CreateSubContainer();

            // Install signals (like EnemyInstaller.InstallSignals)
            subContainer.DeclareSignal<TransformChangedSignal>();
            subContainer.DeclareSignal<PhysicsChangedSignal>();

            // Bind Entity in subcontainer
            subContainer.BindInstance(signal.Entity).AsSingle();

            // Get bullet view from pool (created by factory)
            var bulletView = _bulletPool.Get();

            // Inject dependencies using subcontainer
            subContainer.Inject(bulletView);

            // Register ITickable components directly with TickableManager (like enemies do)
            foreach (var tickableComponent in signal.Entity.GetTickableComponents())
            {
                _tickableManager.Add(tickableComponent);
            }

            bulletView.Initialize();
            _activeBullets.Add(bulletView);
        }

        private void OnBulletDestroyed(BulletDestroyedSignal signal)
        {
            if (signal.Entity == null)
            {
                return;
            }

            // Find and return bullet to pool
            BulletView bulletView = null;
            for (int i = _activeBullets.Count - 1; i >= 0; i--)
            {
                if (_activeBullets[i].Entity == signal.Entity)
                {
                    bulletView = _activeBullets[i];
                    _activeBullets.RemoveAt(i);
                    break;
                }
            }

            if (bulletView != null)
            {
                // Remove ITickable components from TickableManager
                foreach (var tickableComponent in signal.Entity.GetTickableComponents())
                {
                    _tickableManager.Remove(tickableComponent);
                }

                bulletView.Dispose();
                _bulletPool.Return(bulletView);
            }
        }

    }
}


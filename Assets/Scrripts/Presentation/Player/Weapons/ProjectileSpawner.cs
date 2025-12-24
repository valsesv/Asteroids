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
    /// Manager that handles creation and destruction of bullet and laser views
    /// </summary>
    public class ProjectileSpawner : MonoBehaviour, IInitializable, IDisposable
    {
        [SerializeField] private GameObject _bulletPrefab;
        [SerializeField] private GameObject _laserPrefab;
        [SerializeField] private Transform _bulletParent;
        [SerializeField] private Transform _laserParent;

        private SignalBus _signalBus;
        private DiContainer _container;
        private TickableManager _tickableManager;

        // Object pools for projectiles
        private ObjectPool<BulletView> _bulletPool;

        // Factories for creating projectiles (like enemies)
        private ProjectileViewFactory<BulletView> _bulletFactory;
        private ProjectileViewFactory<LaserView> _laserFactory;

        private List<BulletView> _activeBullets = new List<BulletView>();
        private List<LaserView> _activeLasers = new List<LaserView>();

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
            Assert.IsNotNull(_laserPrefab, "LaserPrefab is not assigned in ProjectileSpawner!");
            Assert.IsNotNull(_bulletParent, "BulletParent is not assigned in ProjectileSpawner!");
            Assert.IsNotNull(_laserParent, "LaserParent is not assigned in ProjectileSpawner!");

            _bulletFactory = new ProjectileViewFactory<BulletView>(_container, _bulletPrefab, _bulletParent);
            _bulletPool = new ObjectPool<BulletView>(() => _bulletFactory.Create(null), _bulletParent);
            _laserFactory = new ProjectileViewFactory<LaserView>(_container, _laserPrefab, _laserParent);

            // Subscribe to signals
            _signalBus.Subscribe<BulletCreatedSignal>(OnBulletCreated);
            _signalBus.Subscribe<BulletDestroyedSignal>(OnBulletDestroyed);
            _signalBus.Subscribe<LaserCreatedSignal>(OnLaserCreated);
            _signalBus.Subscribe<LaserDestroyedSignal>(OnLaserDestroyed);
        }

        public void Dispose()
        {
            _signalBus?.Unsubscribe<BulletCreatedSignal>(OnBulletCreated);
            _signalBus?.Unsubscribe<BulletDestroyedSignal>(OnBulletDestroyed);
            _signalBus?.Unsubscribe<LaserCreatedSignal>(OnLaserCreated);
            _signalBus?.Unsubscribe<LaserDestroyedSignal>(OnLaserDestroyed);
        }

        private void OnBulletCreated(BulletCreatedSignal signal)
        {
            if (_bulletPool == null || signal.Entity == null)
            {
                return;
            }

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

        private void OnLaserCreated(LaserCreatedSignal signal)
        {
            if (_laserFactory == null || signal.Entity == null)
            {
                return;
            }

            // Create subcontainer for this laser to track its signals (like EnemyInstaller)
            var subContainer = _container.CreateSubContainer();

            // Install signals (like EnemyInstaller.InstallSignals)
            subContainer.DeclareSignal<TransformChangedSignal>();
            subContainer.DeclareSignal<PhysicsChangedSignal>();

            // Bind Entity in subcontainer
            subContainer.BindInstance(signal.Entity).AsSingle();

            // Create laser view without injection (will be injected via subcontainer)
            var laserView = _laserFactory.Create(null);

            // Inject dependencies using subcontainer (subcontainer inherits parent dependencies like EnemySpawner)
            // GameEntity is already bound in subcontainer, so it will be injected automatically
            subContainer.Inject(laserView);

            // Register ITickable components directly with TickableManager (like enemies do)
            foreach (var tickableComponent in signal.Entity.GetTickableComponents())
            {
                _tickableManager.Add(tickableComponent);
            }

            laserView.Initialize();
            _activeLasers.Add(laserView);
        }

        private void OnLaserDestroyed(LaserDestroyedSignal signal)
        {
            if (signal.Entity == null)
            {
                return;
            }

            // Find and destroy laser
            LaserView laserView = null;
            for (int i = _activeLasers.Count - 1; i >= 0; i--)
            {
                if (_activeLasers[i].Entity == signal.Entity)
                {
                    laserView = _activeLasers[i];
                    _activeLasers.RemoveAt(i);
                    break;
                }
            }

            if (laserView != null)
            {
                // Remove ITickable components from TickableManager
                foreach (var tickableComponent in signal.Entity.GetTickableComponents())
                {
                    _tickableManager.Remove(tickableComponent);
                }

                laserView.Dispose();
                Destroy(laserView.gameObject);
            }
        }
    }
}


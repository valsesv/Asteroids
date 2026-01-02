using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;
using Asteroids.Core.Player;
using UnityEngine.Assertions;

namespace Asteroids.Presentation.Player
{
    public class ProjectileSpawner : MonoBehaviour, IInitializable, IDisposable
    {
        [SerializeField] private GameObject _bulletPrefab;
        [SerializeField] private Transform _bulletParent;

        private SignalBus _signalBus;
        private DiContainer _container;

        private ObjectPool<BulletView> _bulletPool;

        private ProjectileViewFactory<BulletView> _bulletViewFactory;

        private List<BulletView> _activeBullets = new List<BulletView>();

        [Inject]
        public void Construct(SignalBus signalBus, DiContainer container)
        {
            _signalBus = signalBus;
            _container = container;
        }

        public void Initialize()
        {
            Assert.IsNotNull(_bulletPrefab, "BulletPrefab is not assigned in ProjectileSpawner!");
            Assert.IsNotNull(_bulletParent, "BulletParent is not assigned in ProjectileSpawner!");

            _bulletViewFactory = new ProjectileViewFactory<BulletView>(_container, _bulletPrefab, _bulletParent);
            _bulletPool = new ObjectPool<BulletView>(() => _bulletViewFactory.Create(Vector2.zero), _bulletParent);

            _signalBus.Subscribe<BulletShotSignal>(OnBulletShot);
        }

        public void Dispose()
        {
            _signalBus?.Unsubscribe<BulletShotSignal>(OnBulletShot);
        }

        private void OnBulletShot(BulletShotSignal signal)
        {
            var bulletView = _bulletPool.Get();

            bulletView.SetSpawnParameters(signal.Position, signal.Direction);

            _activeBullets.Add(bulletView);
        }

        public void ReturnBullet(BulletView bulletView)
        {
            _activeBullets.Remove(bulletView);
            _bulletPool.Return(bulletView);
        }
    }
}


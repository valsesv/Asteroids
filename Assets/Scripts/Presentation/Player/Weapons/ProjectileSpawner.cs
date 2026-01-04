using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Asteroids.Core.Entity;
using Asteroids.Core.Player;
using UnityEngine.Assertions;

namespace Asteroids.Presentation.Player
{
    public class ProjectileSpawner : MonoBehaviour, IInitializable, IDisposable
    {
        [SerializeField] private GameObject _bulletPrefab;
        [SerializeField] private Transform _bulletParent;

        private DiContainer _container;
        private BulletShootingLogic _bulletShootingLogic;

        private ObjectPool<BulletPresentation> _bulletPool;

        private ProjectilePresentationFactory<BulletPresentation> _bulletPresentationFactory;

        private List<BulletPresentation> _activeBullets = new List<BulletPresentation>();

        [Inject]
        public void Construct(DiContainer container, BulletShootingLogic bulletShootingLogic)
        {
            _container = container;
            _bulletShootingLogic = bulletShootingLogic;
        }

        public void Initialize()
        {
            Assert.IsNotNull(_bulletPrefab, "BulletPrefab is not assigned in ProjectileSpawner!");
            Assert.IsNotNull(_bulletParent, "BulletParent is not assigned in ProjectileSpawner!");

            _bulletPresentationFactory = new ProjectilePresentationFactory<BulletPresentation>(_container, _bulletPrefab, _bulletParent);
            _bulletPool = new ObjectPool<BulletPresentation>(() => _bulletPresentationFactory.Create(Vector2.zero), _bulletParent);

            _bulletShootingLogic.OnBulletShot += OnBulletShot;
        }

        public void Dispose()
        {
            _bulletShootingLogic.OnBulletShot -= OnBulletShot;
        }

        private void OnBulletShot(Vector2 position, Vector2 direction)
        {
            var bulletPresentation = _bulletPool.Get();
            bulletPresentation.SetSpawnParameters(position, direction);

            _activeBullets.Add(bulletPresentation);
        }

        public void ReturnBullet(BulletPresentation bulletPresentation)
        {
            _activeBullets.Remove(bulletPresentation);
            _bulletPool.Return(bulletPresentation);
        }
    }
}
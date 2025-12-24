using UnityEngine;
using Zenject;
using Asteroids.Core.Entity;

namespace Asteroids.Presentation.Player
{
    /// <summary>
    /// Generic factory for creating projectile views (bullets and lasers)
    /// </summary>
    public class ProjectileViewFactory<T> where T : MonoBehaviour
    {
        private readonly DiContainer _container;
        private readonly GameObject _prefab;
        private readonly Transform _parent;

        public ProjectileViewFactory(DiContainer container, GameObject prefab, Transform parent)
        {
            _container = container;
            _prefab = prefab;
            _parent = parent;
        }

        public T Create(GameEntity entity)
        {
            // Create instance without automatic injection if entity is null (for pool creation)
            GameObject instance;
            if (entity == null)
            {
                // Create without injection - will be injected later in OnBulletCreated
                instance = Object.Instantiate(_prefab, _parent);
            }
            else
            {
                // Create with injection when entity is provided
                instance = _container.InstantiatePrefab(_prefab, _parent);
                var projectileView = instance.GetComponent<T>();
                
                // Inject GameEntity explicitly
                var extraArgs = InjectUtil.CreateArgListExplicit(entity);
                _container.InjectExplicit(projectileView, extraArgs);
                
                return projectileView;
            }

            return instance.GetComponent<T>();
        }
    }
}


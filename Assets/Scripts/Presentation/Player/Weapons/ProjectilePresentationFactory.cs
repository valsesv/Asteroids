using UnityEngine;
using Zenject;

namespace Asteroids.Presentation.Player
{
    public class ProjectilePresentationFactory<T> where T : BulletPresentation
    {
        private readonly DiContainer _container;
        private readonly GameObject _prefab;
        private readonly Transform _parent;

        public ProjectilePresentationFactory(DiContainer container, GameObject prefab, Transform parent)
        {
            _container = container;
            _prefab = prefab;
            _parent = parent;
        }

        public T Create(Vector2 position)
        {
            GameObject instance = _container.InstantiatePrefab(_prefab, _parent);
            instance.transform.position = new Vector3(position.x, position.y, 0f);

            var enemy = instance.GetComponent<T>();
            return enemy;
        }
    }
}


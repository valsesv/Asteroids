using UnityEngine;
using Zenject;

namespace Asteroids.Presentation.Enemies
{
    public class EnemyViewFactory<T> where T : EnemyView
    {
        private readonly DiContainer _container;
        private readonly GameObject _prefab;

        public EnemyViewFactory(DiContainer container, GameObject prefab)
        {
            _container = container;
            _prefab = prefab;
        }

        public T Create(Vector2 position)
        {
            GameObject instance = _container.InstantiatePrefab(_prefab);
            instance.transform.position = new Vector3(position.x, position.y, 0f);

            var enemy = instance.GetComponent<T>();
            return enemy;
        }
    }
}


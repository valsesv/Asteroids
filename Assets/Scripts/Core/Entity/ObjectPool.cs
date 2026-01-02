using System.Collections.Generic;
using UnityEngine;

namespace Asteroids.Core.Entity
{
    public class ObjectPool<T> where T : Component
    {
        private readonly List<T> _activeObjects = new List<T>();
        private readonly Stack<T> _inactiveObjects = new Stack<T>();
        private readonly System.Func<T> _factory;
        private readonly Transform _parent;

        private const int InitialSize = 20;

        public ObjectPool(System.Func<T> factory, Transform parent = null, int initialSize = InitialSize)
        {
            _factory = factory;
            _parent = parent;

            if (initialSize > 0)
            {
                PreWarm(initialSize);
            }
        }

        public T Get()
        {
            T obj;
            if (_inactiveObjects.Count > 0)
            {
                obj = _inactiveObjects.Pop();
            }
            else
            {
                obj = _factory();
                if (_parent != null)
                {
                    obj.transform.SetParent(_parent);
                }
            }

            obj.gameObject.SetActive(true);
            _activeObjects.Add(obj);
            return obj;
        }

        public void Return(T obj)
        {
            if (obj == null)
            {
                return;
            }

            if (_activeObjects.Remove(obj))
            {
                obj.gameObject.SetActive(false);
                _inactiveObjects.Push(obj);
            }
        }
        public List<T> ActiveObjects => _activeObjects;

        public void PreWarm(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var obj = _factory();
                if (_parent != null)
                {
                    obj.transform.SetParent(_parent);
                }
                obj.gameObject.SetActive(false);
                _inactiveObjects.Push(obj);
            }
        }

        public void Clear()
        {
            foreach (var obj in _activeObjects)
            {
                if (obj != null)
                {
                    Object.Destroy(obj.gameObject);
                }
            }

            while (_inactiveObjects.Count > 0)
            {
                var obj = _inactiveObjects.Pop();
                if (obj != null)
                {
                    Object.Destroy(obj.gameObject);
                }
            }

            _activeObjects.Clear();
        }
    }
}


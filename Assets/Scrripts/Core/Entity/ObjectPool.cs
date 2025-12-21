using System.Collections.Generic;
using UnityEngine;

namespace Asteroids.Core.Entity
{
    /// <summary>
    /// Generic object pool - similar to List<T> but with pool functionality
    /// </summary>
    public class ObjectPool<T> where T : Component
    {
        private readonly List<T> _activeObjects = new List<T>();
        private readonly Stack<T> _inactiveObjects = new Stack<T>();
        private readonly System.Func<T> _factory;
        private readonly Transform _parent;

        /// <summary>
        /// Create object pool with factory function
        /// </summary>
        /// <param name="factory">Function that creates new objects</param>
        /// <param name="parent">Parent transform for pooled objects (optional)</param>
        public ObjectPool(System.Func<T> factory, Transform parent = null)
        {
            _factory = factory;
            _parent = parent;
        }

        /// <summary>
        /// Get object from pool (or create new if pool is empty)
        /// </summary>
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

        /// <summary>
        /// Return object to pool
        /// </summary>
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

        /// <summary>
        /// Get all active objects (similar to List<T>)
        /// </summary>
        public List<T> ActiveObjects => _activeObjects;

        /// <summary>
        /// Count of active objects
        /// </summary>
        public int Count => _activeObjects.Count;

        /// <summary>
        /// Clear all objects from pool
        /// </summary>
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


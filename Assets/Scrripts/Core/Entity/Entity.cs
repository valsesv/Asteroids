using System;
using System.Collections.Generic;

namespace Asteroids.Core.Entity
{
    /// <summary>
    /// Base entity class that can hold components
    /// Both player and enemy entities can attach components like physics, motion, etc.
    /// </summary>
    public class Entity
    {
        private readonly Dictionary<Type, IComponent> _components = new Dictionary<Type, IComponent>();

        /// <summary>
        /// Add a component to this entity
        /// </summary>
        public void AddComponent<T>(T component) where T : class, IComponent
        {
            _components[typeof(T)] = component;
        }

        /// <summary>
        /// Get a component from this entity
        /// </summary>
        public T GetComponent<T>() where T : class, IComponent
        {
            if (_components.TryGetValue(typeof(T), out var component))
            {
                return component as T;
            }
            return null;
        }

        /// <summary>
        /// Check if entity has a specific component
        /// </summary>
        public bool HasComponent<T>() where T : class, IComponent
        {
            return _components.ContainsKey(typeof(T));
        }

        /// <summary>
        /// Remove a component from this entity
        /// </summary>
        public void RemoveComponent<T>() where T : class, IComponent
        {
            _components.Remove(typeof(T));
        }

        /// <summary>
        /// Get all components
        /// </summary>
        public IEnumerable<IComponent> GetAllComponents()
        {
            return _components.Values;
        }
    }
}


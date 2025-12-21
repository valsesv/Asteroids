using System;
using System.Collections.Generic;
using UnityEngine;
using Asteroids.Core.Entity.Components;
using Zenject;

namespace Asteroids.Core.Entity
{
    /// <summary>
    /// Base entity class that can hold components
    /// Both player and enemy entities can attach components like physics, motion, etc.
    /// Every entity automatically has a TransformComponent by default.
    /// </summary>
    public class GameEntity
    {
        private readonly Dictionary<Type, IComponent> _components = new Dictionary<Type, IComponent>();

        public GameEntity(Vector2 position = default, float rotation = 0f, SignalBus signalBus = null)
        {
            // Every entity has a transform component by default
            AddComponent(new TransformComponent(position, rotation, signalBus));
        }

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

        /// <summary>
        /// Get all tickable components
        /// </summary>
        public IEnumerable<ITickableComponent> GetTickableComponents()
        {
            foreach (var component in _components.Values)
            {
                if (component is ITickableComponent tickable)
                {
                    yield return tickable;
                }
            }
        }
    }
}


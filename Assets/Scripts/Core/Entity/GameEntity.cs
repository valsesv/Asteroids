using System;
using System.Collections.Generic;
using UnityEngine;
using Asteroids.Core.Entity.Components;

namespace Asteroids.Core.Entity
{
    public class GameEntity
    {
        private readonly Dictionary<Type, IComponent> _components = new Dictionary<Type, IComponent>();

        public GameEntity(Vector2 position = default, float rotation = 0f)
        {
            AddComponent(new TransformComponent(position, rotation));
        }

        public void AddComponent<T>(T component) where T : class, IComponent
        {
            _components[typeof(T)] = component;
        }

        public T GetComponent<T>() where T : class, IComponent
        {
            if (_components.TryGetValue(typeof(T), out var component))
            {
                return component as T;
            }

            foreach (var comp in _components.Values)
            {
                if (comp is T result)
                {
                    return result;
                }
            }

            return null;
        }

        public bool HasComponent<T>() where T : class, IComponent
        {
            if (_components.ContainsKey(typeof(T)))
            {
                return true;
            }

            foreach (var component in _components.Values)
            {
                if (component is T)
                {
                    return true;
                }
            }

            return false;
        }

        public void RemoveComponent<T>() where T : class, IComponent
        {
            _components.Remove(typeof(T));
        }

        public IEnumerable<IComponent> GetAllComponents()
        {
            return _components.Values;
        }

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


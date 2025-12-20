using UnityEngine;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;

namespace Asteroids.Core.Player
{
    /// <summary>
    /// Ship model - contains all ship data and state
    /// Uses Entity component system internally
    /// </summary>
    public class ShipModel
    {
        private readonly GameEntity _entity;

        public ShipModel(StartPositionSettings startPositionSettings)
        {
            // Entity automatically creates TransformComponent with initial position/rotation
            _entity = new GameEntity(startPositionSettings.Position, startPositionSettings.Rotation);
            CanControl = true;

            // Add physics component
            _entity.AddComponent(new PhysicsComponent(mass: 1f));
        }

        /// <summary>
        /// Get the underlying entity for component access
        /// </summary>
        public GameEntity Entity => _entity;

        public Vector2 Position
        {
            get => _entity.GetComponent<TransformComponent>().Position;
            set => _entity.GetComponent<TransformComponent>().SetPosition(value);
        }

        public float Rotation
        {
            get => _entity.GetComponent<TransformComponent>().Rotation;
            set => _entity.GetComponent<TransformComponent>().SetRotation(value);
        }

        public Vector2 Velocity
        {
            get
            {
                var physics = _entity.GetComponent<PhysicsComponent>();
                return physics?.Velocity ?? Vector2.zero;
            }
            set
            {
                var physics = _entity.GetComponent<PhysicsComponent>();
                physics?.SetVelocity(value);
            }
        }

        public bool CanControl { get; set; }

        /// <summary>
        /// Get transform component
        /// </summary>
        public TransformComponent GetTransform() => _entity.GetComponent<TransformComponent>();

        /// <summary>
        /// Get physics component
        /// </summary>
        public PhysicsComponent GetPhysics() => _entity.GetComponent<PhysicsComponent>();

        /// <summary>
        /// Get motion component (if added)
        /// </summary>
        public MotionComponent GetMotion() => _entity.GetComponent<MotionComponent>();
    }
}


using System;
using Zenject;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;
using Asteroids.Core.Player;
using Asteroids.Core.Weapons;

namespace Asteroids.Presentation.UI
{
    public class PlayerStatsViewModel : IInitializable, ITickable
    {
        private readonly GameEntity _playerEntity;
        private readonly LaserSettings _laserSettings;

        private TransformComponent _transformComponent;
        private PhysicsComponent _physicsComponent;
        private LaserComponent _laserComponent;

        public float PositionX { get; private set; }
        public float PositionY { get; private set; }
        public float Rotation { get; private set; }
        public float Speed { get; private set; }
        public int LaserCharges { get; private set; }
        public int LaserMaxCharges { get; private set; }
        public float LaserRechargeProgress { get; private set; }

        public event Action OnStatsChanged;

        public PlayerStatsViewModel(GameEntity playerEntity, LaserSettings laserSettings)
        {
            _playerEntity = playerEntity;
            _laserSettings = laserSettings;
        }

        public void Initialize()
        {
            _transformComponent = _playerEntity?.GetComponent<TransformComponent>();
            _physicsComponent = _playerEntity?.GetComponent<PhysicsComponent>();
            _laserComponent = _playerEntity?.GetComponent<LaserComponent>();

            LaserMaxCharges = _laserSettings.MaxCharges;
            LaserCharges = _laserSettings.MaxCharges;
            LaserRechargeProgress = 0f;
        }

        public void Tick()
        {
            PositionX = _transformComponent.Position.x;
            PositionY = _transformComponent.Position.y;
            Rotation = _transformComponent.Rotation;
            Speed = _physicsComponent.Velocity.magnitude;
            LaserCharges = _laserComponent.CurrentCharges;
            LaserMaxCharges = _laserComponent.MaxCharges;
            LaserRechargeProgress = _laserComponent.RechargeProgress;

            OnStatsChanged?.Invoke();
        }
    }
}


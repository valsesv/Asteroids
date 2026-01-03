using System;
using UnityEngine;
using Zenject;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;
using Asteroids.Core.Player;
using Asteroids.Core.Weapons;

namespace Asteroids.Presentation.UI
{
    public class PlayerStatsViewModel : IInitializable, IDisposable, ITickable
    {
        private readonly GameEntity _playerEntity;
        private readonly LaserSettings _laserSettings;

        private TransformComponent _transformComponent;
        private PhysicsComponent _physicsComponent;
        private LaserComponent _laserComponent;

        private float _positionX;
        private float _positionY;
        private float _rotation;
        private float _speed;
        private int _laserCharges;
        private int _laserMaxCharges;
        private float _laserRechargeProgress;

        public float PositionX
        {
            get => _positionX;
            private set
            {
                if (_positionX != value)
                {
                    _positionX = value;
                    OnStatsChanged?.Invoke();
                }
            }
        }

        public float PositionY
        {
            get => _positionY;
            private set
            {
                if (_positionY != value)
                {
                    _positionY = value;
                    OnStatsChanged?.Invoke();
                }
            }
        }

        public float Rotation
        {
            get => _rotation;
            private set
            {
                if (_rotation != value)
                {
                    _rotation = value;
                    OnStatsChanged?.Invoke();
                }
            }
        }

        public float Speed
        {
            get => _speed;
            private set
            {
                if (_speed != value)
                {
                    _speed = value;
                    OnStatsChanged?.Invoke();
                }
            }
        }

        public int LaserCharges
        {
            get => _laserCharges;
            private set
            {
                if (_laserCharges != value)
                {
                    _laserCharges = value;
                    OnStatsChanged?.Invoke();
                }
            }
        }

        public int LaserMaxCharges
        {
            get => _laserMaxCharges;
            private set
            {
                if (_laserMaxCharges != value)
                {
                    _laserMaxCharges = value;
                    OnStatsChanged?.Invoke();
                }
            }
        }

        public float LaserRechargeTime
        {
            get
            {
                if (_laserRechargeProgress >= 1f)
                {
                    return 0f;
                }
                return _laserSettings.RechargeTime * (1f - _laserRechargeProgress);
            }
        }

        public float LaserRechargeProgress
        {
            get => _laserRechargeProgress;
        }

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
            _laserRechargeProgress = 0f;
        }

        public void Dispose()
        {
        }

        public void Tick()
        {
            bool changed = false;

            if (_transformComponent != null)
            {
                if (Mathf.Abs(_positionX - _transformComponent.Position.x) > 0.01f)
                {
                    PositionX = _transformComponent.Position.x;
                    changed = true;
                }
                if (Mathf.Abs(_positionY - _transformComponent.Position.y) > 0.01f)
                {
                    PositionY = _transformComponent.Position.y;
                    changed = true;
                }
                if (Mathf.Abs(_rotation - _transformComponent.Rotation) > 0.1f)
                {
                    Rotation = _transformComponent.Rotation;
                    changed = true;
                }
            }

            if (_physicsComponent != null)
            {
                float currentSpeed = _physicsComponent.Velocity.magnitude;
                if (Mathf.Abs(_speed - currentSpeed) > 0.01f)
                {
                    Speed = currentSpeed;
                    changed = true;
                }
            }

            if (_laserComponent != null)
            {
                if (_laserCharges != _laserComponent.CurrentCharges)
                {
                    LaserCharges = _laserComponent.CurrentCharges;
                    changed = true;
                }
                if (_laserMaxCharges != _laserComponent.MaxCharges)
                {
                    LaserMaxCharges = _laserComponent.MaxCharges;
                    changed = true;
                }
                if (Mathf.Abs(_laserRechargeProgress - _laserComponent.RechargeProgress) > 0.01f)
                {
                    _laserRechargeProgress = _laserComponent.RechargeProgress;
                    changed = true;
                }
            }

            if (changed)
            {
                OnStatsChanged?.Invoke();
            }
        }
    }
}


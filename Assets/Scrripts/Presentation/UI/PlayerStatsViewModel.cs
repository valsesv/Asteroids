using System;
using Zenject;
using Asteroids.Core.Entity.Components;
using Asteroids.Core.Weapons;

namespace Asteroids.Presentation.UI
{
    /// <summary>
    /// ViewModel for player stats display (MVVM pattern)
    /// Non-MonoBehaviour class that manages player stats state for UI
    /// </summary>
    public class PlayerStatsViewModel : IInitializable, IDisposable
    {
        private readonly SignalBus _signalBus;
        private readonly LaserSettings _laserSettings;

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

        public PlayerStatsViewModel(SignalBus signalBus, LaserSettings laserSettings)
        {
            _signalBus = signalBus;
            _laserSettings = laserSettings;
        }

        public void Initialize()
        {
            // Initialize laser charges from settings
            LaserMaxCharges = _laserSettings.MaxCharges;
            LaserCharges = _laserSettings.MaxCharges;
            _laserRechargeProgress = 0f; // No need to charge initially

            // Subscribe to all relevant signals
            _signalBus.Subscribe<TransformChangedSignal>(OnTransformChanged);
            _signalBus.Subscribe<PhysicsChangedSignal>(OnPhysicsChanged);
            _signalBus.Subscribe<LaserChargesChangedSignal>(OnLaserChargesChanged);
        }

        public void Dispose()
        {
            _signalBus?.Unsubscribe<TransformChangedSignal>(OnTransformChanged);
            _signalBus?.Unsubscribe<PhysicsChangedSignal>(OnPhysicsChanged);
            _signalBus?.Unsubscribe<LaserChargesChangedSignal>(OnLaserChargesChanged);
        }

        private void OnTransformChanged(TransformChangedSignal signal)
        {
            PositionX = signal.X;
            PositionY = signal.Y;
            Rotation = signal.Rotation;
        }

        private void OnPhysicsChanged(PhysicsChangedSignal signal)
        {
            Speed = signal.Speed;
        }

        private void OnLaserChargesChanged(LaserChargesChangedSignal signal)
        {
            LaserCharges = signal.CurrentCharges;
            LaserMaxCharges = signal.MaxCharges;
            _laserRechargeProgress = signal.RechargeProgress;
            OnStatsChanged?.Invoke();
        }
    }
}


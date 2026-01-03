using System;
using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Analytics;
using Zenject;
using UnityEngine;
using Asteroids.Core.Game;

namespace Asteroids.Infrastructure.Firebase
{
    public class FirebaseService : IInitializable, IDisposable
    {
        private readonly SignalBus _signalBus;
        private bool _isInitialized;

        public FirebaseService(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        public void Initialize()
        {
            _signalBus.Subscribe<GameStartedSignal>(OnGameStarted);
            InitializeAsync().Forget();
        }

        private async UniTask InitializeAsync()
        {
            try
            {
                var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync().AsUniTask();

                if (dependencyStatus == DependencyStatus.Available)
                {
                    FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
                    _isInitialized = true;
                }
                else
                {
                    Debug.LogError($"[FirebaseService] Could not resolve all Firebase dependencies: {dependencyStatus}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[FirebaseService] Exception during Firebase initialization: {ex.Message}");
            }
        }

        private void OnGameStarted(GameStartedSignal _)
        {
            if (_isInitialized)
            {
                FirebaseAnalytics.LogEvent("game_started");
            }
        }

        public void Dispose()
        {
            _signalBus?.Unsubscribe<GameStartedSignal>(OnGameStarted);
        }
    }
}


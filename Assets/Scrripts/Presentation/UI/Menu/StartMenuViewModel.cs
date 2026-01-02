using System;
using Zenject;
using Asteroids.Core.Game;
using Asteroids.Core.Entity.Components;

namespace Asteroids.Presentation.UI
{
    /// <summary>
    /// ViewModel for start menu display (MVVM pattern)
    /// Non-MonoBehaviour class that manages start menu state for UI
    /// </summary>
    public class StartMenuViewModel : IInitializable, IDisposable
    {
        private readonly SignalBus _signalBus;
        private readonly GameController _gameController;

        private bool _isVisible = true;

        public bool IsVisible
        {
            get => _isVisible;
            private set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    OnVisibilityChanged?.Invoke(_isVisible);
                }
            }
        }

        public event Action<bool> OnVisibilityChanged;

        public StartMenuViewModel(SignalBus signalBus, GameController gameController)
        {
            _signalBus = signalBus;
            _gameController = gameController;
        }

        public void Initialize()
        {
            // Subscribe to game state signals
            _signalBus.Subscribe<GameStartedSignal>(OnGameStarted);
            _signalBus.Subscribe<GameOverSignal>(OnGameOver);

            // Initialize visibility based on current game state
            IsVisible = _gameController.CurrentState != GameController.GameState.Playing;
        }

        public void Dispose()
        {
            _signalBus?.Unsubscribe<GameStartedSignal>(OnGameStarted);
            _signalBus?.Unsubscribe<GameOverSignal>(OnGameOver);
        }

        /// <summary>
        /// Called when user clicks to start the game
        /// </summary>
        public void OnStartClicked()
        {
            _gameController.StartGame();
        }

        private void OnGameStarted(GameStartedSignal signal)
        {
            IsVisible = false;
        }

        private void OnGameOver(GameOverSignal signal)
        {
            IsVisible = true;
        }
    }
}


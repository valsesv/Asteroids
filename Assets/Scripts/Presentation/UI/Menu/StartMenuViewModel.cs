using System;
using Zenject;
using Asteroids.Core.Game;

namespace Asteroids.Presentation.UI
{
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
            _signalBus.Subscribe<GameStartedSignal>(OnGameStarted);
            _signalBus.Subscribe<GameOverSignal>(OnGameOver);

            IsVisible = _gameController.CurrentState != GameController.GameState.Playing;
        }

        public void Dispose()
        {
            _signalBus?.Unsubscribe<GameStartedSignal>(OnGameStarted);
            _signalBus?.Unsubscribe<GameOverSignal>(OnGameOver);
        }

        public void OnStartClicked()
        {
            _gameController.StartGame();
        }

        private void OnGameStarted(GameStartedSignal _)
        {
            IsVisible = false;
        }

        private void OnGameOver(GameOverSignal _)
        {
            IsVisible = true;
        }
    }
}


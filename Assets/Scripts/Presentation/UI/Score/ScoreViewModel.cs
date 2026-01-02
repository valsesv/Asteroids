using System;
using Zenject;
using Asteroids.Core.Score;
using Asteroids.Core.Entity.Components;

namespace Asteroids.Presentation.UI
{
    /// <summary>
    /// ViewModel for score display (MVVM pattern)
    /// Non-MonoBehaviour class that manages score state for UI
    /// </summary>
    public class ScoreViewModel : IInitializable, IDisposable
    {
        private readonly SignalBus _signalBus;
        private readonly ScoreService _scoreService;

        private int _currentScore;
        public int CurrentScore
        {
            get => _currentScore;
            private set
            {
                if (_currentScore != value)
                {
                    _currentScore = value;
                    OnScoreChanged?.Invoke(_currentScore);
                }
            }
        }

        public event Action<int> OnScoreChanged;

        public ScoreViewModel(SignalBus signalBus, ScoreService scoreService)
        {
            _signalBus = signalBus;
            _scoreService = scoreService;
        }

        public void Initialize()
        {
            // Subscribe to score changes
            _signalBus.Subscribe<ScoreChangedSignal>(OnScoreChangedSignal);

            // Initialize with current score from service
            CurrentScore = _scoreService.CurrentScore;
        }

        public void Dispose()
        {
            _signalBus?.Unsubscribe<ScoreChangedSignal>(OnScoreChangedSignal);
        }

        private void OnScoreChangedSignal(ScoreChangedSignal signal)
        {
            CurrentScore = signal.CurrentScore;
        }
    }
}


using System;
using Zenject;
using Asteroids.Core.Score;

namespace Asteroids.Presentation.UI
{
    public class ScoreViewModel : IInitializable, IDisposable
    {
        public int CurrentScore { get; private set; }
        public event Action<int> OnScoreChanged;

        private readonly ScoreService _scoreService;

        public ScoreViewModel(ScoreService scoreService)
        {
            _scoreService = scoreService;
        }

        public void Initialize()
        {
            _scoreService.OnScoreChanged += OnScoreChangedHandler;
            CurrentScore = _scoreService.CurrentScore;
        }

        public void Dispose()
        {
            if (_scoreService != null)
            {
                _scoreService.OnScoreChanged -= OnScoreChangedHandler;
            }
        }

        private void OnScoreChangedHandler(int currentScore)
        {
            CurrentScore = currentScore;
            OnScoreChanged?.Invoke(CurrentScore);
        }
    }
}
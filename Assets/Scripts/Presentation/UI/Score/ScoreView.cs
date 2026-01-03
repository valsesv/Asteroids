using System;
using UnityEngine;
using TMPro;
using Zenject;
using UnityEngine.Assertions;

namespace Asteroids.Presentation.UI
{
    /// <summary>
    /// View for displaying score in UI (MVVM pattern)
    /// MonoBehaviour that binds to ScoreViewModel
    /// </summary>
    public class ScoreView : MonoBehaviour, IInitializable, IDisposable
    {
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private string _scoreFormat = "Score: {0}";

        private ScoreViewModel _viewModel;

        [Inject]
        public void Construct(ScoreViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public void Initialize()
        {
            Assert.IsNotNull(_scoreText, "ScoreText is not assigned in ScoreView!");

            // Subscribe to ViewModel changes
            _viewModel.OnScoreChanged += UpdateScoreDisplay;

            // Initialize display with current score
            UpdateScoreDisplay(_viewModel.CurrentScore);
        }

        public void Dispose()
        {
            if (_viewModel != null)
            {
                _viewModel.OnScoreChanged -= UpdateScoreDisplay;
            }
        }

        private void UpdateScoreDisplay(int score)
        {
            _scoreText.text = string.Format(_scoreFormat, score);
        }
    }
}


using System;
using UnityEngine;
using TMPro;
using Zenject;

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
            if (_scoreText == null)
            {
                Debug.LogError("ScoreText is not assigned in ScoreView!");
                return;
            }

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
            if (_scoreText != null)
            {
                _scoreText.text = string.Format(_scoreFormat, score);
            }
        }
    }
}


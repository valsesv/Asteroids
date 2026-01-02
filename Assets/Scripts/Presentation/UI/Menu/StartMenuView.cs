using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;

namespace Asteroids.Presentation.UI
{
    /// <summary>
    /// View for start menu display (MVVM pattern)
    /// MonoBehaviour that binds to StartMenuViewModel
    /// Handles user interactions (clicks) - visibility is managed by GameUIView
    /// </summary>
    public class StartMenuView : MonoBehaviour, IInitializable, IDisposable
    {
        [SerializeField] private Button _startButton;

        private StartMenuViewModel _viewModel;

        [Inject]
        public void Construct(StartMenuViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public void Initialize()
        {
            // Setup start button
            if (_startButton != null)
            {
                _startButton.onClick.AddListener(OnStartButtonClicked);
            }
        }

        public void Dispose()
        {
            if (_startButton != null)
            {
                _startButton.onClick.RemoveListener(OnStartButtonClicked);
            }
        }

        private void OnStartButtonClicked()
        {
            if (_viewModel != null && _viewModel.IsVisible)
            {
                _viewModel.OnStartClicked();
            }
        }
    }
}


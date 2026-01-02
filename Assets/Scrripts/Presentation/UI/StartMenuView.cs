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
    /// </summary>
    public class StartMenuView : MonoBehaviour, IInitializable, IDisposable
    {
        [SerializeField] private GameObject _menuPanel;
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

            // Subscribe to ViewModel changes
            _viewModel.OnVisibilityChanged += UpdateVisibility;

            // Initial visibility update
            UpdateVisibility(_viewModel.IsVisible);
        }

        public void Dispose()
        {
            if (_startButton != null)
            {
                _startButton.onClick.RemoveListener(OnStartButtonClicked);
            }

            if (_viewModel != null)
            {
                _viewModel.OnVisibilityChanged -= UpdateVisibility;
            }
        }

        private void OnStartButtonClicked()
        {
            _viewModel?.OnStartClicked();
        }

        private void UpdateVisibility(bool isVisible)
        {
            if (_menuPanel != null)
            {
                _menuPanel.SetActive(isVisible);
            }
        }
    }
}


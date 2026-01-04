using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UnityEngine.Assertions;

namespace Asteroids.Presentation.UI
{
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
            Assert.IsNotNull(_startButton, "StartButton is not assigned in StartMenuView!");
            _startButton.onClick.AddListener(OnStartButtonClicked);
        }

        public void Dispose()
        {
            _startButton.onClick.RemoveListener(OnStartButtonClicked);
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
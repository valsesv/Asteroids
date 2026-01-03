using System;
using UnityEngine;
using Zenject;
using Asteroids.Core.Entity.Components;

namespace Asteroids.Presentation.UI
{
    /// <summary>
    /// View for managing game UI visibility (health, stats, score) and menu panel
    /// Shows game UI when game starts, hides when game over
    /// Shows menu panel when game over, hides when game starts
    /// </summary>
    public class GameUIView : MonoBehaviour, IInitializable, IDisposable
    {
        [SerializeField] private GameObject _gameUIPanel;
        [SerializeField] private GameObject _menuPanel;

        private SignalBus _signalBus;

        [Inject]
        public void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        public void Initialize()
        {
            // Subscribe to game state signals
            _signalBus.Subscribe<GameStartedSignal>(OnGameStarted);
            _signalBus.Subscribe<GameOverSignal>(OnGameOver);

            // Initially hide game UI and show menu (game hasn't started yet)
            SetGameUIVisible(false);
            SetMenuVisible(true);
        }

        public void Dispose()
        {
            _signalBus?.Unsubscribe<GameStartedSignal>(OnGameStarted);
            _signalBus?.Unsubscribe<GameOverSignal>(OnGameOver);
        }

        private void OnGameStarted(GameStartedSignal _)
        {
            SetGameUIVisible(true);
            SetMenuVisible(false);
        }

        private void OnGameOver(GameOverSignal _)
        {
            SetGameUIVisible(false);
            SetMenuVisible(true);
        }

        private void SetGameUIVisible(bool isVisible)
        {
            if (_gameUIPanel != null)
            {
                _gameUIPanel.SetActive(isVisible);
            }
        }

        private void SetMenuVisible(bool isVisible)
        {
            if (_menuPanel != null)
            {
                _menuPanel.SetActive(isVisible);
            }
        }
    }
}


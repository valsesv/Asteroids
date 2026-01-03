using System;
using UnityEngine;
using Zenject;
using Asteroids.Core.Entity.Components;
using UnityEngine.Assertions;

namespace Asteroids.Presentation.UI
{
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
            Assert.IsNotNull(_gameUIPanel, "GameUIPanel is not assigned in GameUIView!");
            Assert.IsNotNull(_menuPanel, "MenuPanel is not assigned in GameUIView!");

            _signalBus.Subscribe<GameStartedSignal>(OnGameStarted);
            _signalBus.Subscribe<GameOverSignal>(OnGameOver);

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
            _gameUIPanel.SetActive(isVisible);
        }

        private void SetMenuVisible(bool isVisible)
        {
            _menuPanel.SetActive(isVisible);
        }
    }
}


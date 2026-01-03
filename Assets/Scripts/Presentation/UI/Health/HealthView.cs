using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Zenject;
using Asteroids.Core.Player;
using UnityEngine.Assertions;

namespace Asteroids.Presentation.UI
{
    public class HealthView : MonoBehaviour, IInitializable, IDisposable
    {
        [SerializeField] private Image _heartPrefab;
        [SerializeField] private float _fadeOutDuration = 0.5f;

        private HealthViewModel _viewModel;
        private DiContainer _container;
        private HealthSettings _healthSettings;
        private List<Image> _heartImages = new List<Image>();
        private Dictionary<Image, Tween> _activeTweens = new Dictionary<Image, Tween>();

        [Inject]
        public void Construct(HealthViewModel viewModel, DiContainer container, HealthSettings healthSettings)
        {
            _viewModel = viewModel;
            _container = container;
            _healthSettings = healthSettings;
        }

        public void Initialize()
        {
            Assert.IsNotNull(_heartPrefab, "HeartPrefab is not assigned in HealthView!");

            _viewModel.OnHealthChanged += OnHealthChanged;

            int maxHealthCount = Mathf.RoundToInt(_healthSettings.MaxHealth);
            for (int i = 0; i < maxHealthCount; i++)
            {
                CreateHeart();
            }
        }

        public void Dispose()
        {
            foreach (var tween in _activeTweens.Values)
            {
                tween?.Kill();
            }
            _activeTweens.Clear();
            _heartImages.Clear();

            if (_viewModel != null)
            {
                _viewModel.OnHealthChanged -= OnHealthChanged;
            }
        }

        private void OnHealthChanged(float currentHealth)
        {
            UpdateHealthDisplay(currentHealth);
        }

        private void UpdateHealthDisplay(float currentHealth)
        {
            int healthCount = Mathf.RoundToInt(currentHealth);

            for (int i = 0; i < _heartImages.Count; i++)
            {
                bool shouldBeVisible = i < healthCount;
                bool isVisible = _heartImages[i].gameObject.activeSelf;

                if (shouldBeVisible && !isVisible)
                {
                    ResetHeart(_heartImages[i]);
                }
                else if (!shouldBeVisible && isVisible)
                {
                    FadeOutHeart(_heartImages[i]);
                }
            }
        }

        private void CreateHeart()
        {
            Image heartInstance = _container.InstantiatePrefabForComponent<Image>(_heartPrefab, transform);
            ResetHeart(heartInstance);

            _heartImages.Add(heartInstance);
        }

        private void FadeOutHeart(Image heartImage)
        {
            if (heartImage == null || !heartImage.gameObject.activeSelf)
            {
                return;
            }

            if (_activeTweens.TryGetValue(heartImage, out var existingTween))
            {
                existingTween?.Kill();
            }

            var tween = DOTween.To(() => heartImage.color, x => heartImage.color = x, new Color(heartImage.color.r, heartImage.color.g, heartImage.color.b, 0f), _fadeOutDuration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    heartImage.gameObject.SetActive(false);
                    _activeTweens.Remove(heartImage);
                });

            _activeTweens[heartImage] = tween;
        }

        private void ResetHeart(Image heartImage)
        {
            if (heartImage == null)
            {
                return;
            }

            if (_activeTweens.TryGetValue(heartImage, out var existingTween))
            {
                existingTween?.Kill();
                _activeTweens.Remove(heartImage);
            }

            heartImage.color = new Color(1f, 1f, 1f, 1f);
            heartImage.gameObject.SetActive(true);
        }
    }
}


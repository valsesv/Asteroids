using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Zenject;
using Asteroids.Core.Player;

namespace Asteroids.Presentation.UI
{
    /// <summary>
    /// View for displaying player health as heart sprites (MVVM pattern)
    /// MonoBehaviour that binds to HealthViewModel
    /// Creates heart instances from prefab and uses Horizontal Layout Group for positioning
    /// </summary>
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
            if (_heartPrefab == null)
            {
                Debug.LogError("HeartPrefab is not assigned in HealthView! Please assign heart prefab in Inspector.");
                return;
            }

            // Subscribe to ViewModel changes
            _viewModel.OnHealthChanged += OnHealthChanged;

            // Create all hearts based on max health from settings
            int maxHealthCount = Mathf.RoundToInt(_healthSettings.MaxHealth);
            for (int i = 0; i < maxHealthCount; i++)
            {
                CreateHeart();
            }
        }

        public void Dispose()
        {
            // Kill all active tweens
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

        /// <summary>
        /// Update heart display based on current health
        /// Only changes state of hearts that need to change
        /// </summary>
        private void UpdateHealthDisplay(float currentHealth)
        {
            int healthCount = Mathf.RoundToInt(currentHealth);

            // Update all hearts - only change state of those that need to change
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

        /// <summary>
        /// Create a single heart instance from prefab
        /// </summary>
        private void CreateHeart()
        {
            if (_heartPrefab == null || _container == null)
            {
                Debug.LogError("Cannot create heart: HeartPrefab or Container is null!");
                return;
            }

            // Instantiate heart prefab as child of this GameObject
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

            // Kill existing tween for this heart if any
            if (_activeTweens.TryGetValue(heartImage, out var existingTween))
            {
                existingTween?.Kill();
            }

            // Create fade out tween
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

            // Kill existing tween for this heart if any
            if (_activeTweens.TryGetValue(heartImage, out var existingTween))
            {
                existingTween?.Kill();
                _activeTweens.Remove(heartImage);
            }

            // Reset heart to full visibility and activate
            heartImage.color = new Color(1f, 1f, 1f, 1f);
            heartImage.gameObject.SetActive(true);
        }
    }
}


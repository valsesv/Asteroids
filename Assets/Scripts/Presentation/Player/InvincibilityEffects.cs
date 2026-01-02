using UnityEngine;
using DG.Tweening;
using UnityEngine.Assertions;

namespace Asteroids.Presentation.Player
{
    [System.Serializable]
    public class InvincibilityEffects
    {
        [SerializeField] private ParticleSystem _particleSystem;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private float _flickerDuration = 0.1f;

        private Tween _flickerTween;
        private Color _originalColor;
        private bool _isActive;

        /// <summary>
        /// Initialize effects - cache original sprite color
        /// </summary>
        public void Initialize()
        {
            Assert.IsNotNull(_particleSystem);
            Assert.IsNotNull(_spriteRenderer);
            _originalColor = _spriteRenderer.color;
        }

        /// <summary>
        /// Start invincibility effects
        /// </summary>
        public void StartEffects()
        {
            if (_isActive)
            {
                return;
            }

            _isActive = true;
            _particleSystem.Play();
            StartFlicker();
        }

        /// <summary>
        /// Stop invincibility effects
        /// </summary>
        public void StopEffects()
        {
            if (_isActive == false)
            {
                return;
            }

            _isActive = false;
            _particleSystem.Stop();
            StopFlicker();
        }

        /// <summary>
        /// Start sprite flicker animation using DOTween
        /// </summary>
        private void StartFlicker()
        {
            StopFlicker();
            Color targetColor = new Color(_originalColor.r, _originalColor.g, _originalColor.b, 0.3f);
            _flickerTween = DOTween.To(
                () => _spriteRenderer.color,
                x => _spriteRenderer.color = x,
                targetColor,
                _flickerDuration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine)
                .SetTarget(_spriteRenderer);
        }

        /// <summary>
        /// Stop sprite flicker animation
        /// </summary>
        private void StopFlicker()
        {
            _flickerTween?.Kill();
            _spriteRenderer.color = _originalColor;
        }

        /// <summary>
        /// Cleanup on destroy
        /// </summary>
        public void Dispose()
        {
            StopEffects();
        }
    }
}


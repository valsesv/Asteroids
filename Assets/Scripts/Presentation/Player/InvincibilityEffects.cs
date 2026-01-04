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

        public void Initialize()
        {
            Assert.IsNotNull(_particleSystem);
            Assert.IsNotNull(_spriteRenderer);

            _originalColor = _spriteRenderer.color;
        }

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

        private void StopFlicker()
        {
            _flickerTween?.Kill();
            _spriteRenderer.color = _originalColor;
        }

        public void Dispose()
        {
            StopEffects();
        }
    }
}
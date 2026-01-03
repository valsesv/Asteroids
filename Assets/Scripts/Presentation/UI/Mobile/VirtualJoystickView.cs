using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Assertions;

namespace Asteroids.Presentation.UI
{
    public class VirtualJoystickView : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private RectTransform _joystickBackground;
        [SerializeField] private RectTransform _joystickHandle;

        private Vector2 _direction = Vector2.zero;
        private bool _isActive = false;
        private float _joystickRange;

        public Vector2 Direction => _direction;

        public bool IsActive => _isActive;

        private void Awake()
        {
            Assert.IsNotNull(_joystickBackground, "JoystickBackground is not assigned in VirtualJoystickView!");
            Assert.IsNotNull(_joystickHandle, "JoystickHandle is not assigned in VirtualJoystickView!");

            CalculateJoystickRange();
            _joystickHandle.anchoredPosition = Vector2.zero;
        }

        private void CalculateJoystickRange()
        {
            float backgroundRadius = Mathf.Min(
                _joystickBackground.rect.width,
                _joystickBackground.rect.height
            ) * 0.5f;

            float handleRadius = Mathf.Min(
                _joystickHandle.rect.width,
                _joystickHandle.rect.height
            ) * 0.5f;

            _joystickRange = backgroundRadius - handleRadius;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _isActive = true;
            OnDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            Canvas canvas = _joystickBackground.GetComponentInParent<Canvas>();
            Camera cam = canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay
                ? canvas.worldCamera
                : null;

            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _joystickBackground,
                eventData.position,
                cam,
                out localPoint);

            _direction = Vector2.ClampMagnitude(localPoint, _joystickRange);
            _direction /= _joystickRange;

            _joystickHandle.anchoredPosition = _direction * _joystickRange;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isActive = false;
            _direction = Vector2.zero;
            _joystickHandle.anchoredPosition = Vector2.zero;
        }

        public void Reset()
        {
            _isActive = false;
            _direction = Vector2.zero;
            _joystickHandle.anchoredPosition = Vector2.zero;
        }
    }
}


using UnityEngine;
using UnityEngine.EventSystems;

namespace Asteroids.Presentation.UI
{
    /// <summary>
    /// Virtual joystick for mobile input
    /// Supports drag and touch input
    /// </summary>
    public class VirtualJoystickView : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private RectTransform _joystickBackground;
        [SerializeField] private RectTransform _joystickHandle;
        [SerializeField] private float _joystickRange = 50f;

        private Vector2 _direction = Vector2.zero;
        private bool _isActive = false;

        /// <summary>
        /// Normalized direction vector (-1 to 1 for both axes)
        /// </summary>
        public Vector2 Direction => _direction;

        /// <summary>
        /// Whether joystick is currently being used
        /// </summary>
        public bool IsActive => _isActive;

        private void Awake()
        {
            if (_joystickHandle != null)
            {
                _joystickHandle.anchoredPosition = Vector2.zero;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _isActive = true;
            OnDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_joystickBackground == null || _joystickHandle == null)
                return;

            // Get canvas camera (null for Screen Space Overlay)
            Canvas canvas = _joystickBackground.GetComponentInParent<Canvas>();
            Camera cam = canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay
                ? canvas.worldCamera
                : null;

            // Convert screen position to local position in joystick background
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _joystickBackground,
                eventData.position,
                cam,
                out localPoint);

            // Clamp to joystick range
            _direction = Vector2.ClampMagnitude(localPoint, _joystickRange);
            _direction /= _joystickRange; // Normalize to -1..1

            // Update handle position
            _joystickHandle.anchoredPosition = _direction * _joystickRange;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isActive = false;
            _direction = Vector2.zero;

            if (_joystickHandle != null)
            {
                _joystickHandle.anchoredPosition = Vector2.zero;
            }
        }

        /// <summary>
        /// Reset joystick to center position
        /// </summary>
        public void Reset()
        {
            _isActive = false;
            _direction = Vector2.zero;

            if (_joystickHandle != null)
            {
                _joystickHandle.anchoredPosition = Vector2.zero;
            }
        }
    }
}


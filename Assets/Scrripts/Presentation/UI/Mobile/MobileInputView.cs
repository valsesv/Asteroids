using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Asteroids.Presentation.UI
{
    /// <summary>
    /// Mobile input view that manages shoot buttons
    /// Buttons can be held down for continuous shooting
    /// </summary>
    public class MobileInputView : MonoBehaviour
    {
        [SerializeField] private Button _shootBulletButton;
        [SerializeField] private Button _shootLaserButton;

        private bool _isShootBulletPressed = false;
        private bool _isShootLaserPressed = false;
        private bool _wasShootLaserPressedLastFrame = false;
        private bool _laserPressedThisFrame = false;

        /// <summary>
        /// Whether bullet shoot button is currently pressed
        /// </summary>
        public bool IsShootBulletPressed => _isShootBulletPressed;

        /// <summary>
        /// Whether laser shoot button was pressed this frame (for single shot)
        /// Returns true only on the frame when button is first pressed
        /// </summary>
        public bool IsShootLaserPressed => _laserPressedThisFrame;

        private void Awake()
        {
            SetupButton(_shootBulletButton, OnShootBulletDown, OnShootBulletUp);
            SetupButton(_shootLaserButton, OnShootLaserDown, OnShootLaserUp);
        }

        private void SetupButton(Button button, UnityEngine.Events.UnityAction onDown, UnityEngine.Events.UnityAction onUp)
        {
            if (button == null)
                return;

            // Add EventTrigger for pointer down/up events
            var eventTrigger = button.gameObject.GetComponent<EventTrigger>();
            if (eventTrigger == null)
            {
                eventTrigger = button.gameObject.AddComponent<EventTrigger>();
            }

            // Pointer Down
            var pointerDown = new EventTrigger.Entry();
            pointerDown.eventID = EventTriggerType.PointerDown;
            pointerDown.callback.AddListener((data) => { onDown?.Invoke(); });
            eventTrigger.triggers.Add(pointerDown);

            // Pointer Up
            var pointerUp = new EventTrigger.Entry();
            pointerUp.eventID = EventTriggerType.PointerUp;
            pointerUp.callback.AddListener((data) => { onUp?.Invoke(); });
            eventTrigger.triggers.Add(pointerUp);

            // Also handle pointer exit (when finger moves outside button)
            var pointerExit = new EventTrigger.Entry();
            pointerExit.eventID = EventTriggerType.PointerExit;
            pointerExit.callback.AddListener((data) => { onUp?.Invoke(); });
            eventTrigger.triggers.Add(pointerExit);
        }

        private void OnShootBulletDown()
        {
            _isShootBulletPressed = true;
        }

        private void OnShootBulletUp()
        {
            _isShootBulletPressed = false;
        }

        private void OnShootLaserDown()
        {
            _isShootLaserPressed = true;
            // Set flag for this frame only if it wasn't pressed last frame
            if (!_wasShootLaserPressedLastFrame)
            {
                _laserPressedThisFrame = true;
            }
        }

        private void OnShootLaserUp()
        {
            _isShootLaserPressed = false;
        }

        private void LateUpdate()
        {
            // Reset laser press flag after reading
            _laserPressedThisFrame = false;
            _wasShootLaserPressedLastFrame = _isShootLaserPressed;
        }

        /// <summary>
        /// Reset all input states
        /// </summary>
        public void Reset()
        {
            _isShootBulletPressed = false;
            _isShootLaserPressed = false;
            _wasShootLaserPressedLastFrame = false;
            _laserPressedThisFrame = false;
        }
    }
}


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

        /// <summary>
        /// Whether bullet shoot button is currently pressed
        /// </summary>
        public bool IsShootBulletPressed => _isShootBulletPressed;

        /// <summary>
        /// Whether laser shoot button is currently pressed
        /// </summary>
        public bool IsShootLaserPressed => _isShootLaserPressed;

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
        }

        private void OnShootLaserUp()
        {
            _isShootLaserPressed = false;
        }

        /// <summary>
        /// Reset all input states
        /// </summary>
        public void Reset()
        {
            _isShootBulletPressed = false;
            _isShootLaserPressed = false;
        }
    }
}


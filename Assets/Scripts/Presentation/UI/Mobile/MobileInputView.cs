using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Assertions;

namespace Asteroids.Presentation.UI
{
    public class MobileInputView : MonoBehaviour
    {
        [SerializeField] private Button _shootBulletButton;
        [SerializeField] private Button _shootLaserButton;

        private bool _isShootBulletPressed = false;
        private bool _isShootLaserPressed = false;

        public bool IsShootBulletPressed => _isShootBulletPressed;

        public bool IsShootLaserPressed => _isShootLaserPressed;

        private void Awake()
        {
            Assert.IsNotNull(_shootBulletButton, "ShootBulletButton is not assigned in MobileInputView!");
            Assert.IsNotNull(_shootLaserButton, "ShootLaserButton is not assigned in MobileInputView!");

            SetupButton(_shootBulletButton, OnShootBulletDown, OnShootBulletUp);
            SetupButton(_shootLaserButton, OnShootLaserDown, OnShootLaserUp);
        }

        private void SetupButton(Button button, UnityEngine.Events.UnityAction onDown, UnityEngine.Events.UnityAction onUp)
        {
            var eventTrigger = button.gameObject.GetComponent<EventTrigger>();
            if (eventTrigger == null)
            {
                eventTrigger = button.gameObject.AddComponent<EventTrigger>();
            }

            var pointerDown = new EventTrigger.Entry();
            pointerDown.eventID = EventTriggerType.PointerDown;
            pointerDown.callback.AddListener((data) => { onDown?.Invoke(); });
            eventTrigger.triggers.Add(pointerDown);

            var pointerUp = new EventTrigger.Entry();
            pointerUp.eventID = EventTriggerType.PointerUp;
            pointerUp.callback.AddListener((data) => { onUp?.Invoke(); });
            eventTrigger.triggers.Add(pointerUp);

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

        public void Reset()
        {
            _isShootBulletPressed = false;
            _isShootLaserPressed = false;
        }
    }
}


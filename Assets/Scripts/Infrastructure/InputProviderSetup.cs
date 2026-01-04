using UnityEngine;
using Asteroids.Presentation.UI;

namespace Asteroids.Infrastructure
{
    public class InputProviderSetup
    {
        public InputProviderSetupResult Setup(
            GameObject mobileInputPanelPrefab,
            Canvas gameCanvas)
        {
            bool shouldUseMobileInput = Application.isEditor || Application.platform == RuntimePlatform.Android;
            bool isEditor = Application.isEditor;

            VirtualJoystickView joystickView = null;
            MobileInputView mobileInputView = null;

            if (shouldUseMobileInput && mobileInputPanelPrefab != null && gameCanvas != null)
            {
                GameObject mobileInputInstance = Object.Instantiate(mobileInputPanelPrefab, gameCanvas.transform);

                joystickView = mobileInputInstance.GetComponentInChildren<VirtualJoystickView>();
                mobileInputView = mobileInputInstance.GetComponentInChildren<MobileInputView>();
            }

            InputProviderType providerType;

            if (isEditor && joystickView != null && mobileInputView != null)
            {
                providerType = InputProviderType.Combined;
            }
            else if (shouldUseMobileInput && joystickView != null && mobileInputView != null)
            {
                providerType = InputProviderType.VirtualJoystick;
            }
            else
            {
                providerType = InputProviderType.Keyboard;
            }

            return new InputProviderSetupResult
            {
                ProviderType = providerType,
                JoystickView = joystickView,
                MobileInputView = mobileInputView
            };
        }
    }

    public class InputProviderSetupResult
    {
        public InputProviderType ProviderType { get; set; }
        public VirtualJoystickView JoystickView { get; set; }
        public MobileInputView MobileInputView { get; set; }
    }

    public enum InputProviderType
    {
        Keyboard,
        VirtualJoystick,
        Combined
    }
}
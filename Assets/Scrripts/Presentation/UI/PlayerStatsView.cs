using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;

namespace Asteroids.Presentation.UI
{
    /// <summary>
    /// View for displaying player stats in UI (MVVM pattern)
    /// MonoBehaviour that binds to PlayerStatsViewModel
    /// </summary>
    public class PlayerStatsView : MonoBehaviour, IInitializable, IDisposable
    {
        [SerializeField] private TextMeshProUGUI _positionText;
        [SerializeField] private TextMeshProUGUI _rotationText;
        [SerializeField] private TextMeshProUGUI _speedText;
        [SerializeField] private TextMeshProUGUI _laserChargesText;
        [SerializeField] private Image _laserRechargeImage;

        [SerializeField] private string _positionFormat = "Position: ({0:F1}, {1:F1})";
        [SerializeField] private string _rotationFormat = "Rotation: {0:F1}°";
        [SerializeField] private string _speedFormat = "Speed: {0:F2}";
        [SerializeField] private string _laserChargesFormat = "Laser Charges: {0}/{1}";

        private PlayerStatsViewModel _viewModel;

        [Inject]
        public void Construct(PlayerStatsViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public void Initialize()
        {
            // Setup laser recharge image if assigned
            if (_laserRechargeImage != null)
            {
                if (_laserRechargeImage.type != Image.Type.Filled)
                {
                    _laserRechargeImage.type = Image.Type.Filled;
                    _laserRechargeImage.fillMethod = Image.FillMethod.Horizontal;
                }
            }

            // Subscribe to ViewModel changes
            _viewModel.OnStatsChanged += UpdateStatsDisplay;

            // Initial update
            UpdateStatsDisplay();
        }

        public void Dispose()
        {
            if (_viewModel != null)
            {
                _viewModel.OnStatsChanged -= UpdateStatsDisplay;
            }
        }

        private void UpdateStatsDisplay()
        {
            if (_positionText != null)
            {
                _positionText.text = string.Format(_positionFormat, _viewModel.PositionX, _viewModel.PositionY);
            }

            if (_rotationText != null)
            {
                _rotationText.text = string.Format(_rotationFormat, _viewModel.Rotation);
            }

            if (_speedText != null)
            {
                _speedText.text = string.Format(_speedFormat, _viewModel.Speed);
            }

            if (_laserChargesText != null)
            {
                _laserChargesText.text = string.Format(_laserChargesFormat, _viewModel.LaserCharges, _viewModel.LaserMaxCharges);
            }

            if (_laserRechargeImage != null)
            {
                // Display recharge progress as fill amount (0 to 1)
                _laserRechargeImage.fillAmount = _viewModel.LaserRechargeProgress;
            }
        }
    }
}


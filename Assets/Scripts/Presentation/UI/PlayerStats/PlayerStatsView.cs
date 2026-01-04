using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;
using UnityEngine.Assertions;

namespace Asteroids.Presentation.UI
{
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
            Assert.IsNotNull(_positionText, "PositionText is not assigned in PlayerStatsView!");
            Assert.IsNotNull(_rotationText, "RotationText is not assigned in PlayerStatsView!");
            Assert.IsNotNull(_speedText, "SpeedText is not assigned in PlayerStatsView!");
            Assert.IsNotNull(_laserChargesText, "LaserChargesText is not assigned in PlayerStatsView!");
            Assert.IsNotNull(_laserRechargeImage, "LaserRechargeImage is not assigned in PlayerStatsView!");
            Assert.IsTrue(_laserRechargeImage.type == Image.Type.Filled, "LaserRechargeImage is not of type Filled!");

            _viewModel.OnStatsChanged += UpdateStatsDisplay;

            UpdateStatsDisplay();
        }

        public void Dispose()
        {
            _viewModel.OnStatsChanged -= UpdateStatsDisplay;
        }

        private void UpdateStatsDisplay()
        {
            _positionText.text = string.Format(_positionFormat, _viewModel.PositionX, _viewModel.PositionY);
            _rotationText.text = string.Format(_rotationFormat, _viewModel.Rotation);
            _speedText.text = string.Format(_speedFormat, _viewModel.Speed);
            _laserChargesText.text = string.Format(_laserChargesFormat, _viewModel.LaserCharges, _viewModel.LaserMaxCharges);
            _laserRechargeImage.fillAmount = _viewModel.LaserRechargeProgress;
        }
    }
}
using System;
using Zenject;
using YandexMobileAds;
using YandexMobileAds.Base;
using UnityEngine;
using Asteroids.Core.Game;

namespace Asteroids.Infrastructure.Ads
{
    public class AdService : IInitializable, IDisposable
    {
        private readonly SignalBus _signalBus;
        private readonly AdSettings _adSettings;

        private InterstitialAdLoader _interstitialAdLoader;
        private Interstitial _currentInterstitial;
        private bool _isLoading;
        private bool _isAdReady;

        public AdService(SignalBus signalBus, AdSettings adSettings)
        {
            _signalBus = signalBus;
            _adSettings = adSettings;
        }

        public void Initialize()
        {
            _signalBus.Subscribe<GameOverSignal>(OnGameOver);

            _interstitialAdLoader = new InterstitialAdLoader();
            _interstitialAdLoader.OnAdLoaded += HandleAdLoaded;
            _interstitialAdLoader.OnAdFailedToLoad += HandleAdFailedToLoad;

            LoadAd();
        }

        public void Dispose()
        {
            _signalBus?.Unsubscribe<GameOverSignal>(OnGameOver);

            CleanupAd();

            if (_interstitialAdLoader == null)
            {
                return;
            }

            _interstitialAdLoader.OnAdLoaded -= HandleAdLoaded;
            _interstitialAdLoader.OnAdFailedToLoad -= HandleAdFailedToLoad;
            _interstitialAdLoader = null;
        }

        private void LoadAd()
        {
            if (_isLoading || _isAdReady)
            {
                return;
            }

            string adUnitId = GetCurrentAdUnitId();

            if (string.IsNullOrEmpty(adUnitId))
            {
                Debug.LogWarning("[AdService] Ad Unit ID is empty");
                return;
            }

            try
            {
                _isLoading = true;
                _isAdReady = false;
                var config = new AdRequestConfiguration.Builder(adUnitId).Build();
                _interstitialAdLoader.LoadAd(config);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AdService] Failed to load ad: {ex.Message}");
                _isLoading = false;
            }
        }

        private void ShowAd()
        {
            if (_isAdReady && _currentInterstitial != null)
            {
                _currentInterstitial.Show();
            }
            else
            {
                LoadAd();
            }
        }

        private void OnGameOver(GameOverSignal _)
        {
            ShowAd();
        }

        private string GetCurrentAdUnitId()
        {
#if UNITY_IOS
            return _adSettings.InterstitialAdUnitIdIOS;
#elif UNITY_ANDROID
            return _adSettings.InterstitialAdUnitIdAndroid;
#else
            return _adSettings.InterstitialAdUnitIdAndroid;
#endif
        }

        private void CleanupAd()
        {
            if (_currentInterstitial == null)
            {
                return;
            }

            _currentInterstitial.OnAdShown -= HandleAdShown;
            _currentInterstitial.OnAdDismissed -= HandleAdDismissed;
            _currentInterstitial.OnAdFailedToShow -= HandleAdFailedToShow;
            _currentInterstitial.Destroy();
            _currentInterstitial = null;
        }

        private void HandleAdLoaded(object sender, InterstitialAdLoadedEventArgs args)
        {
            CleanupAd();

            _currentInterstitial = args.Interstitial;
            _isLoading = false;
            _isAdReady = true;

            _currentInterstitial.OnAdShown += HandleAdShown;
            _currentInterstitial.OnAdDismissed += HandleAdDismissed;
            _currentInterstitial.OnAdFailedToShow += HandleAdFailedToShow;
        }

        private void HandleAdFailedToLoad(object _, AdFailedToLoadEventArgs args)
        {
            Debug.LogWarning($"[AdService] Failed to load ad: {args.Message}");

            _isLoading = false;
            _isAdReady = false;
        }

        private void HandleAdShown(object _, EventArgs __)
        {
            Debug.Log("[AdService] Ad shown");
        }

        private void HandleAdDismissed(object _, EventArgs __)
        {
            _isAdReady = false;

            CleanupAd();
            LoadAd();
        }

        private void HandleAdFailedToShow(object _, AdFailureEventArgs args)
        {
            Debug.LogWarning($"[AdService] Failed to show ad: {args.Message}");
            _isAdReady = false;

            CleanupAd();
            LoadAd();
        }
    }
}
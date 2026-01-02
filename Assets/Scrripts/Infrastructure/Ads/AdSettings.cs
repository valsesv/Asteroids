namespace Asteroids.Infrastructure.Ads
{
    /// <summary>
    /// Settings for ads configuration
    /// Can be loaded from JSON or set via ScriptableObject
    /// </summary>
    [System.Serializable]
    public class AdSettings
    {
        public string InterstitialAdUnitIdAndroid = "demo-interstitial-yandex";
        public string InterstitialAdUnitIdIOS = "demo-interstitial-yandex";
    }
}


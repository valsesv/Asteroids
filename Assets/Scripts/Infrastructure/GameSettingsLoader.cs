using UnityEngine;
using UnityEngine.Assertions;
using Utils.JsonLoader;
using Asteroids.Core.Player;
using Asteroids.Core.Enemies;
using Asteroids.Core.Score;

namespace Asteroids.Infrastructure
{
    public class GameSettingsLoader
    {
        private const string PlayerSettingsFileName = "player_settings.json";
        private const string EnemySettingsFileName = "enemy_settings.json";
        private const string ScoreSettingsFileName = "score_settings.json";

        private readonly JsonLoader _jsonLoader;

        public GameSettingsLoader()
        {
            _jsonLoader = new JsonLoader();
        }

        public LoadedGameSettings LoadSettings()
        {
            var playerSettings = _jsonLoader.LoadFromStreamingAssets<PlayerSettings>(PlayerSettingsFileName);
            Assert.IsNotNull(playerSettings, "Failed to load player settings from JSON!");
            Assert.IsNotNull(playerSettings.Movement, "Movement settings are null in player settings!");
            Assert.IsNotNull(playerSettings.StartPosition, "Start position settings are null in player settings!");
            Assert.IsNotNull(playerSettings.Health, "Health settings are null in player settings!");
            Assert.IsNotNull(playerSettings.Weapon, "Weapon settings are null in player settings!");
            Assert.IsNotNull(playerSettings.Weapon.Laser, "Laser settings are null in weapon settings!");

            var enemySettings = _jsonLoader.LoadFromStreamingAssets<EnemySettings>(EnemySettingsFileName);
            Assert.IsNotNull(enemySettings, "Failed to load enemy settings from JSON!");

            var scoreSettings = _jsonLoader.LoadFromStreamingAssets<ScoreSettings>(ScoreSettingsFileName);
            Assert.IsNotNull(scoreSettings, "Failed to load score settings from JSON!");

            return new LoadedGameSettings
            {
                PlayerSettings = playerSettings,
                EnemySettings = enemySettings,
                ScoreSettings = scoreSettings
            };
        }
    }

    public class LoadedGameSettings
    {
        public PlayerSettings PlayerSettings { get; set; }
        public EnemySettings EnemySettings { get; set; }
        public ScoreSettings ScoreSettings { get; set; }
    }
}


namespace Asteroids.Core.Entity.Components
{
    public class GameStartedSignal
    {
    }

    public class GameOverSignal
    {
    }

    public class EnemyDestroyedSignal
    {
        public GameEntity Entity { get; set; }
    }

    public class ScoreChangedSignal
    {
        public int CurrentScore { get; set; }
        public int PointsAdded { get; set; }
    }
}


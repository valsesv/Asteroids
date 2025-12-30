namespace Asteroids.Core.Enemies
{
    /// <summary>
    /// Component for Fragment-specific data and behavior
    /// Fragments are spawned when asteroids are destroyed by bullets
    /// </summary>
    public class FragmentComponent : EnemyComponent
    {
        public FragmentComponent() : base(EnemyType.Fragment)
        {
        }
    }
}


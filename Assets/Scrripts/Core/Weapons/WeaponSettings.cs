namespace Asteroids.Core.Player
{
    /// <summary>
    /// Weapon settings for bullets
    /// </summary>
    public class WeaponSettings
    {
        public BulletSettings Bullet;
    }

    /// <summary>
    /// Bullet weapon settings
    /// </summary>
    public class BulletSettings
    {
        public float Speed = 10f;
        public float FireRate = 2f; // Shots per second
        public float Lifetime = 3f; // How long bullet exists before auto-destroy
    }
}


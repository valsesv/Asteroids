namespace Asteroids.Core.Player
{
    /// <summary>
    /// Weapon settings for bullets and laser
    /// </summary>
    public class WeaponSettings
    {
        public BulletSettings Bullet;
        public LaserSettings Laser;
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

    /// <summary>
    /// Laser weapon settings
    /// </summary>
    public class LaserSettings
    {
        public int MaxCharges = 3;
        public float RechargeTime = 5.0f; // Time to recharge one charge
        public float Duration = 0.5f; // How long laser stays active
        public float Width = 0.2f; // Width of laser beam
        public float Range = 1000f; // Maximum range of laser
    }
}


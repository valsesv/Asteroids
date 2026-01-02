namespace Asteroids.Core.Weapons
{
    public class WeaponSettings
    {
        public BulletSettings Bullet;
        public LaserSettings Laser;
    }

    public class BulletSettings
    {
        public float Speed = 10f;
        public float FireRate = 2f;
        public float Lifetime = 3f;
    }

    public class LaserSettings
    {
        public int MaxCharges = 3;
        public float RechargeTime = 5.0f;
        public float Duration = 0.5f;
        public float Width = 0.2f;
        public float Range = 1000f;
    }
}


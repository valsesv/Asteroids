namespace Asteroids.Core.Weapons
{
    public class WeaponSettings
    {
        public BulletSettings Bullet;
        public LaserSettings Laser;
    }

    public class BulletSettings
    {
        public float Speed;
        public float FireRate;
        public float Lifetime;
    }

    public class LaserSettings
    {
        public int MaxCharges;
        public float RechargeTime;
        public float Duration;
        public float Width;
        public float Range;
    }
}
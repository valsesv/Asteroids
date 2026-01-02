using UnityEngine;
using Zenject;
using Asteroids.Core.Entity;

namespace Asteroids.Core.Enemies
{
    public static class FragmentFactory
    {
        public static GameEntity CreateFragment(
            Vector2 position,
            float rotation,
            SignalBus signalBus,
            float speed,
            ScreenBounds screenBounds)
        {
            var entity = EnemyFactory.CreateEnemy(EnemyType.Fragment, position, rotation, signalBus, screenBounds);

            var fragmentComponent = new FragmentComponent();
            entity.AddComponent(fragmentComponent);

            var movement = new AsteroidMovement(entity, speed);
            entity.AddComponent(movement);

            return entity;
        }
    }
}


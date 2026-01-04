using UnityEngine;
using Asteroids.Core.Entity;
using Cysharp.Threading.Tasks;
using Zenject;
using UnityEngine.Assertions;

namespace Asteroids.Presentation.Effects
{
    public class ParticleEffectSpawner : MonoBehaviour, IInitializable
    {
        [SerializeField] private GameObject _explosionPrefab;
        [SerializeField] private Transform _particleParent;

        private ObjectPool<ParticleSystem> _explosionPool;

        public void Initialize()
        {
            Assert.IsNotNull(_explosionPrefab, "ExplosionPrefab is not assigned in ParticleEffectSpawner!");
            Assert.IsNotNull(_particleParent, "ParticleParent is not assigned in ParticleEffectSpawner!");

            _explosionPool = new ObjectPool<ParticleSystem>(
                () => CreateParticleSystem(),
                _particleParent,
                initialSize: 10
            );
        }

        private ParticleSystem CreateParticleSystem()
        {
            var instance = Instantiate(_explosionPrefab, _particleParent);
            var particleSystem = instance.GetComponent<ParticleSystem>();

            if (particleSystem == null)
            {
                particleSystem = instance.GetComponentInChildren<ParticleSystem>();
            }
            
            return particleSystem;
        }

        public void SpawnExplosion(Vector2 position)
        {
            var particleSystem = _explosionPool.Get();
            particleSystem.transform.position = new Vector3(position.x, position.y, 0f);
            particleSystem.Clear();
            particleSystem.Play();

            _ = ReturnParticleSystemAfterDelay(particleSystem);
        }

        private async UniTask ReturnParticleSystemAfterDelay(ParticleSystem particleSystem)
        {
            while (particleSystem != null && particleSystem.isPlaying)
            {
                await UniTask.Yield();
            }

            await UniTask.WaitForSeconds(0.1f);

            if (particleSystem != null)
            {
                _explosionPool.Return(particleSystem);
            }
        }
    }
}
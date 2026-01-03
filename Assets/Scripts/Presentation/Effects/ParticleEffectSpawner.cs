using UnityEngine;
using Asteroids.Core.Entity;
using Cysharp.Threading.Tasks;
using Zenject;

namespace Asteroids.Presentation.Effects
{
    public class ParticleEffectSpawner : MonoBehaviour, IInitializable
    {
        [SerializeField] private GameObject _explosionPrefab;
        [SerializeField] private Transform _particleParent;

        private ObjectPool<ParticleSystem> _explosionPool;

        public void Initialize()
        {
            if (_explosionPrefab != null && _particleParent != null)
            {
                _explosionPool = new ObjectPool<ParticleSystem>(
                    () => CreateParticleSystem(),
                    _particleParent,
                    initialSize: 10
                );
            }
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
            if (_explosionPool == null)
            {
                return;
            }

            var particleSystem = _explosionPool.Get();
            particleSystem.transform.position = new Vector3(position.x, position.y, 0f);
            particleSystem.Clear();
            particleSystem.Play();

            ReturnParticleSystemAfterDelay(particleSystem).Forget();
        }

        private async UniTask ReturnParticleSystemAfterDelay(ParticleSystem particleSystem)
        {
            while (particleSystem != null && particleSystem.isPlaying)
            {
                await UniTask.Yield();
            }

            await UniTask.Delay(System.TimeSpan.FromSeconds(0.1f));

            if (particleSystem != null)
            {
                _explosionPool.Return(particleSystem);
            }
        }
    }
}


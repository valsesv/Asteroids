using UnityEngine;
using Asteroids.Core.Entity;
using Cysharp.Threading.Tasks;
using Zenject;

namespace Asteroids.Presentation.Effects
{
    /// <summary>
    /// Spawner for explosion particle effects using object pooling
    /// </summary>
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

        /// <summary>
        /// Spawn explosion effect at given position
        /// </summary>
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

            // Return to pool after effect finishes using UniTask
            ReturnParticleSystemAfterDelay(particleSystem).Forget();
        }

        private async UniTaskVoid ReturnParticleSystemAfterDelay(ParticleSystem particleSystem)
        {
            // Wait for particle system to finish playing
            while (particleSystem != null && particleSystem.isPlaying)
            {
                await UniTask.Yield();
            }

            // Wait a bit more to ensure particles have fully finished
            await UniTask.Delay(System.TimeSpan.FromSeconds(0.1f));

            if (particleSystem != null)
            {
                _explosionPool.Return(particleSystem);
            }
        }
    }
}


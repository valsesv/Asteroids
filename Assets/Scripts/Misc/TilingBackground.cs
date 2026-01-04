using UnityEngine;

namespace Asteroids.Misc
{
    public class TilingBackground : MonoBehaviour
    {
        [SerializeField] private Renderer _renderer;
        [SerializeField] private float _speed;

        private Vector2 _offset;

        private void Update()
        {
            _offset.y += _speed * Time.deltaTime;
            _renderer.material.mainTextureOffset = _offset;
        }
    }
}
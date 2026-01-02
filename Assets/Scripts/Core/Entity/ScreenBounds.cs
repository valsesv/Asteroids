using UnityEngine;
using Zenject;

namespace Asteroids.Core.Entity
{
    /// <summary>
    /// Service for getting screen boundaries based on camera
    /// </summary>
    public class ScreenBounds
    {
        private readonly Camera _camera;

        public ScreenBounds(Camera camera)
        {
            _camera = camera;
        }

        public float Bottom => -ExtentHeight;
        public float Top => ExtentHeight;
        public float Left => -ExtentWidth;
        public float Right => ExtentWidth;

        public float ExtentHeight => _camera.orthographicSize;
        public float Height => ExtentHeight * 2.0f;
        public float ExtentWidth => _camera.aspect * _camera.orthographicSize;
        public float Width => ExtentWidth * 2.0f;
    }
}


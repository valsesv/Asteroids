using UnityEngine;
using Asteroids.Core.Enemies;

namespace Asteroids.Presentation.Enemies
{
    /// <summary>
    /// Base enemy view - MonoBehaviour that represents enemy in the scene
    /// </summary>
    public abstract class EnemyView : MonoBehaviour
    {
        protected EnemyModel Model;

        public void Initialize(EnemyModel model)
        {
            Model = model;
        }
    }
}


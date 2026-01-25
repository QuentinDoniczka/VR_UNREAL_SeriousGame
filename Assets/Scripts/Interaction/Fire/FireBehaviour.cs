using System;
using UnityEngine;

namespace Interaction.Fire
{
    [RequireComponent(typeof(Collider))]
    public class FireBehaviour : MonoBehaviour
    {
        [Header("Growth Settings")]
        [SerializeField] private float growthSpeed = 0.1f;
        [SerializeField] private float maxScale = 1f;

        [Header("Kill Settings")]
        [SerializeField] private float minScaleToKill = 0.2f;

        private float _spawnTime;

        public float TimeAlive => Time.time - _spawnTime;
        public event Action<FireBehaviour> OnExtinguished;
        public event Action<FireBehaviour> OnExpired;

        private void Awake()
        {
            _spawnTime = Time.time;
        }

        private void Update()
        {
            Grow();
        }

        public void ApplyExtinguish(float power)
        {
            Shrink(power);
        }

        private void Grow()
        {
            if (transform.localScale.x >= maxScale) return;

            transform.localScale += Vector3.one * (growthSpeed * Time.deltaTime);

            if (transform.localScale.x > maxScale)
            {
                transform.localScale = Vector3.one * maxScale;
            }
        }

        private void Shrink(float power)
        {
            transform.localScale -= Vector3.one * (power * Time.deltaTime);

            if (transform.localScale.x <= minScaleToKill)
            {
                Kill();
            }
        }

        private void Kill()
        {
            OnExtinguished?.Invoke(this);
            Destroy(gameObject);
        }

        public void Expire()
        {
            OnExpired?.Invoke(this);
            Destroy(gameObject);
        }
    }
}

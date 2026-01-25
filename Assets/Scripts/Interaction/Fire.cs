using UnityEngine;

namespace Interaction
{
    [RequireComponent(typeof(Collider))]
    public class Fire : MonoBehaviour
    {
        [Header("Growth Settings")]
        [SerializeField] private float growthSpeed = 0.1f;
        [SerializeField] private float maxScale = 1f;

        [Header("Kill Settings")]
        [SerializeField] private float minScaleToKill = 0.2f;

        private void Update()
        {
            Grow();
        }

        private void OnTriggerStay(Collider other)
        {
            var sprayCone = other.GetComponent<SprayCone>();
            if (sprayCone == null || sprayCone.Extinguisher == null) return;

            Shrink(sprayCone.Extinguisher.ExtinguishingPower);
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
            Debug.Log("[Fire] Fire extinguished!");
            Destroy(gameObject);
        }
    }
}

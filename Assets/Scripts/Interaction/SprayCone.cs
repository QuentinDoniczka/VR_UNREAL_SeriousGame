using Interaction.Fire;
using UnityEngine;

namespace Interaction
{
    [RequireComponent(typeof(Collider))]
    public class SprayCone : MonoBehaviour
    {
        private FireExtinguisher extinguisher;

        public FireExtinguisher Extinguisher => extinguisher;

        private void Awake()
        {
            extinguisher = GetComponentInParent<FireExtinguisher>();

            if (extinguisher == null)
            {
                Debug.LogError("[SprayCone] No FireExtinguisher found in parent.", this);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (extinguisher == null) return;
            if (!other.TryGetComponent<FireBehaviour>(out var fire)) return;

            fire.TryApplyExtinguish(extinguisher.Type, extinguisher.ExtinguishingPower);
        }
    }
}

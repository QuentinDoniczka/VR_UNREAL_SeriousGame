using UnityEngine;

namespace Interaction
{
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
    }
}

using UnityEngine;

namespace Interaction.Fire
{
    public class FireSpawnPoint : MonoBehaviour
    {
        [Header("Cooldown")]
        [SerializeField] private float cooldownAfterExtinguish = 5f;

        private bool _isOccupied;
        private float _cooldownTimer;

        public bool IsAvailable => !_isOccupied && _cooldownTimer <= 0f;

        private void Update()
        {
            if (_cooldownTimer > 0f)
            {
                _cooldownTimer -= Time.deltaTime;
            }
        }

        public Vector3 GetSpawnPosition() => transform.position;

        public Quaternion GetSpawnRotation() => transform.rotation;

        public void MarkOccupied() => _isOccupied = true;

        public void MarkFree()
        {
            _isOccupied = false;
            _cooldownTimer = cooldownAfterExtinguish;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = IsAvailable ? Color.green : Color.red;
            Gizmos.DrawWireSphere(transform.position, 0.3f);
        }
    }
}

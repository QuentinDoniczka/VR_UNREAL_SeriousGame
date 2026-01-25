using UnityEngine;

namespace Interaction.Fire
{
    public class FireSpawnZone : MonoBehaviour, IFireSpawnLocation
    {
        [Header("Zone Size (X/Z plane)")]
        [SerializeField] private Vector2 size = new Vector2(5f, 5f);

        [Header("Capacity")]
        [SerializeField] private int maxFiresInZone = 3;

        private int _currentFireCount;

        public bool IsAvailable => _currentFireCount < maxFiresInZone;
        public bool CanSpawnMultiple => true;

        public Vector3 GetSpawnPosition()
        {
            float randomX = Random.Range(-size.x / 2f, size.x / 2f);
            float randomZ = Random.Range(-size.y / 2f, size.y / 2f);

            Vector3 localOffset = new Vector3(randomX, 0f, randomZ);
            return transform.position + transform.rotation * localOffset;
        }

        public Quaternion GetSpawnRotation() => transform.rotation;

        public void MarkOccupied() => _currentFireCount++;

        public void MarkFree() => _currentFireCount = Mathf.Max(0, _currentFireCount - 1);

        private void OnDrawGizmos()
        {
            Gizmos.color = IsAvailable ? new Color(0f, 1f, 0f, 0.3f) : new Color(1f, 0f, 0f, 0.3f);
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(Vector3.zero, new Vector3(size.x, 0.1f, size.y));

            Gizmos.color = IsAvailable ? Color.green : Color.red;
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(size.x, 0.1f, size.y));
        }
    }
}

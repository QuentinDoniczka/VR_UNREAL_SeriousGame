using System.Collections.Generic;
using UI;
using UnityEngine;

namespace Interaction.Fire
{
    public class FireManager : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [SerializeField] private GameObject firePrefab;
        [SerializeField] private int maxActiveFires = 5;
        [SerializeField] private float spawnInterval = 10f;
        [SerializeField] private float initialDelay = 3f;

        [Header("Fire Settings")]
        [SerializeField] private float initialScale = 0.3f;
        [SerializeField] private float maxFireDuration = 120f;

        private readonly List<FireSpawnPoint> _spawnPoints = new();
        private readonly Dictionary<FireBehaviour, FireSpawnPoint> _activeFires = new();

        private float _spawnTimer;
        private bool _isInitialized;

        private void Start()
        {
            _spawnTimer = initialDelay;
            CollectSpawnPoints();
            _isInitialized = true;
        }

        private void Update()
        {
            if (!_isInitialized) return;

            _spawnTimer -= Time.deltaTime;

            if (_spawnTimer <= 0f && _activeFires.Count < maxActiveFires)
            {
                TrySpawnFire();
                _spawnTimer = spawnInterval;
            }

            CheckExpiredFires();
        }

        private void CheckExpiredFires()
        {
            List<FireBehaviour> toExpire = new();

            foreach (var fire in _activeFires.Keys)
            {
                if (fire.TimeAlive >= maxFireDuration)
                {
                    toExpire.Add(fire);
                }
            }

            foreach (var fire in toExpire)
            {
                fire.Expire();
            }
        }

        private void CollectSpawnPoints()
        {
            _spawnPoints.AddRange(FindObjectsByType<FireSpawnPoint>(FindObjectsSortMode.None));
            Debug.Log($"[FireManager] Found {_spawnPoints.Count} spawn points.");
        }

        private void TrySpawnFire()
        {
            FireSpawnPoint point = GetAvailablePoint();
            if (point == null)
            {
                Debug.Log("[FireManager] No available spawn points.");
                return;
            }

            SpawnFireAt(point);
        }

        private FireSpawnPoint GetAvailablePoint()
        {
            List<FireSpawnPoint> available = _spawnPoints.FindAll(p => p.IsAvailable);
            if (available.Count == 0) return null;

            return available[Random.Range(0, available.Count)];
        }

        private void SpawnFireAt(FireSpawnPoint point)
        {
            Vector3 position = point.GetSpawnPosition();
            Quaternion rotation = point.GetSpawnRotation();

            GameObject fireObj = Instantiate(firePrefab, position, rotation);
            fireObj.transform.localScale = Vector3.one * initialScale;

            FireBehaviour fire = fireObj.GetComponent<FireBehaviour>();
            if (fire == null)
            {
                Debug.LogError("[FireManager] Fire prefab missing FireBehaviour component!");
                Destroy(fireObj);
                return;
            }

            FireType randomType = GetRandomFireType();
            fire.SetFireType(randomType);

            fire.OnExtinguished += HandleFireExtinguished;
            fire.OnExpired += HandleFireExpired;
            point.MarkOccupied();
            _activeFires.Add(fire, point);

            Debug.Log($"[FireManager] Spawned {randomType} fire at {position}. Active fires: {_activeFires.Count}");
        }

        private void HandleFireExtinguished(FireBehaviour fire)
        {
            UnregisterFire(fire);

            var scoreHUD = ScoreHUD.Instance;
            if (scoreHUD != null)
                scoreHUD.AddScore();
        }

        private void HandleFireExpired(FireBehaviour fire)
        {
            UnregisterFire(fire);
            Debug.Log($"[FireManager] Fire expired after {maxFireDuration}s! Active fires: {_activeFires.Count}");
        }

        private void UnregisterFire(FireBehaviour fire)
        {
            fire.OnExtinguished -= HandleFireExtinguished;
            fire.OnExpired -= HandleFireExpired;

            if (_activeFires.TryGetValue(fire, out FireSpawnPoint point))
            {
                point.MarkFree();
                _activeFires.Remove(fire);
            }
        }

        public int ActiveFireCount => _activeFires.Count;

        private static FireType GetRandomFireType()
        {
            var values = System.Enum.GetValues(typeof(FireType));
            return (FireType)values.GetValue(Random.Range(0, values.Length));
        }

        public void ForceSpawnFire()
        {
            if (_activeFires.Count >= maxActiveFires) return;
            TrySpawnFire();
        }
    }
}

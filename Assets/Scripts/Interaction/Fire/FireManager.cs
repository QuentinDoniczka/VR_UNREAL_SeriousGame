using System.Collections.Generic;
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

        [Header("Location Selection")]
        [SerializeField, Range(0f, 1f)] private float zoneSelectionWeight = 0.5f;

        private readonly List<FireSpawnPoint> _spawnPoints = new();
        private readonly List<FireSpawnZone> _spawnZones = new();
        private readonly Dictionary<FireBehaviour, IFireSpawnLocation> _activeFires = new();

        private float _spawnTimer;
        private bool _isInitialized;

        private void Start()
        {
            _spawnTimer = initialDelay;
            CollectSpawnLocations();
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

        private void CollectSpawnLocations()
        {
            _spawnPoints.AddRange(FindObjectsByType<FireSpawnPoint>(FindObjectsSortMode.None));
            _spawnZones.AddRange(FindObjectsByType<FireSpawnZone>(FindObjectsSortMode.None));

            Debug.Log($"[FireManager] Found {_spawnPoints.Count} spawn points and {_spawnZones.Count} spawn zones.");
        }

        private void TrySpawnFire()
        {
            IFireSpawnLocation location = SelectSpawnLocation();
            if (location == null) return;

            SpawnFireAt(location);
        }

        private IFireSpawnLocation SelectSpawnLocation()
        {
            bool tryZoneFirst = Random.value < zoneSelectionWeight;

            if (tryZoneFirst)
            {
                IFireSpawnLocation zone = GetAvailableZone();
                if (zone != null) return zone;

                return GetAvailablePoint();
            }

            IFireSpawnLocation point = GetAvailablePoint();
            if (point != null) return point;

            return GetAvailableZone();
        }

        private IFireSpawnLocation GetAvailablePoint()
        {
            List<FireSpawnPoint> available = _spawnPoints.FindAll(p => p.IsAvailable);
            if (available.Count == 0) return null;

            return available[Random.Range(0, available.Count)];
        }

        private IFireSpawnLocation GetAvailableZone()
        {
            List<FireSpawnZone> available = _spawnZones.FindAll(z => z.IsAvailable);
            if (available.Count == 0) return null;

            return available[Random.Range(0, available.Count)];
        }

        private void SpawnFireAt(IFireSpawnLocation location)
        {
            Vector3 position = location.GetSpawnPosition();
            Quaternion rotation = location.GetSpawnRotation();

            GameObject fireObj = Instantiate(firePrefab, position, rotation);
            fireObj.transform.localScale = Vector3.one * initialScale;

            FireBehaviour fire = fireObj.GetComponent<FireBehaviour>();
            if (fire == null)
            {
                Debug.LogError("[FireManager] Fire prefab missing FireBehaviour component!");
                Destroy(fireObj);
                return;
            }

            fire.OnExtinguished += HandleFireExtinguished;
            fire.OnExpired += HandleFireExpired;
            location.MarkOccupied();
            _activeFires.Add(fire, location);

            Debug.Log($"[FireManager] Spawned fire at {position}. Active fires: {_activeFires.Count}");
        }

        private void HandleFireExtinguished(FireBehaviour fire)
        {
            UnregisterFire(fire);
            Debug.Log($"[FireManager] Fire extinguished. Active fires: {_activeFires.Count}");
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

            if (_activeFires.TryGetValue(fire, out IFireSpawnLocation location))
            {
                location.MarkFree();
                _activeFires.Remove(fire);
            }
        }

        public int ActiveFireCount => _activeFires.Count;

        public void ForceSpawnFire()
        {
            if (_activeFires.Count >= maxActiveFires) return;
            TrySpawnFire();
        }
    }
}

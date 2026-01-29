using System;
using Interaction;
using Interaction.Inspection;
using UnityEngine;

namespace Interaction.Fire
{
    [RequireComponent(typeof(Collider))]
    public class FireBehaviour : MonoBehaviour, IInspectable
    {
        [Header("Growth Settings")]
        [SerializeField] private float growthSpeed = 0.1f;
        [SerializeField] private float maxScale = 1f;

        [Header("Fire Type")]
        [SerializeField] private FireType fireType = FireType.SolidMaterial;

        [Header("Kill Settings")]
        [SerializeField] private float minScaleToKill = 0.2f;

        [Header("Base Anchor")]
        [Tooltip("Height of the visual mesh (used to keep base anchored when scaling)")]
        [SerializeField] private float meshHeight = 1f;

        private float _spawnTime;
        private float _baseY;

        public float TimeAlive => Time.time - _spawnTime;
        public FireType FireType => fireType;

        public void SetFireType(FireType type)
        {
            fireType = type;
        }

        public bool IsExtinguisherAllowed(ExtinguisherType extinguisherType)
        {
            return FireCompatibility.IsExtinguisherAllowed(fireType, extinguisherType);
        }

        public event Action<FireBehaviour> OnExtinguished;
        public event Action<FireBehaviour> OnExpired;

        private void Awake()
        {
            _spawnTime = Time.time;
            _baseY = transform.position.y - (transform.localScale.y * meshHeight * 0.5f);
        }

        private void Update()
        {
            Grow();
        }

        public void ApplyExtinguish(float power)
        {
            Shrink(power);
        }

        public bool TryApplyExtinguish(ExtinguisherType extinguisherType, float power)
        {
            if (!IsExtinguisherAllowed(extinguisherType))
            {
                FireSafetyMessages.LogIncompatibility(fireType, extinguisherType);
                return false;
            }

            Shrink(power);
            return true;
        }

        private void Grow()
        {
            if (transform.localScale.x >= maxScale) return;

            transform.localScale += Vector3.one * (growthSpeed * Time.deltaTime);

            if (transform.localScale.x > maxScale)
            {
                transform.localScale = Vector3.one * maxScale;
            }

            AnchorToBase();
        }

        private void Shrink(float power)
        {
            transform.localScale -= Vector3.one * (power * Time.deltaTime);

            if (transform.localScale.x <= minScaleToKill)
            {
                Kill();
                return;
            }

            AnchorToBase();
        }

        private void AnchorToBase()
        {
            var pos = transform.position;
            pos.y = _baseY + (transform.localScale.y * meshHeight * 0.5f);
            transform.position = pos;
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

        public string GetInspectionName()
        {
            return "Feu";
        }

        public string GetInspectionDetails()
        {
            return fireType switch
            {
                FireType.Electrical => "Électrique",
                FireType.SolidMaterial => "Matière solide",
                FireType.FlammableLiquid => "Liquide inflammable",
                _ => "Inconnu"
            };
        }
    }
}

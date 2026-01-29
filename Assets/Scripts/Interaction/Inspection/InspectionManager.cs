using Core.Managers;
using UnityEngine;

namespace Interaction.Inspection
{
    public class InspectionManager : MonoBehaviour
    {
        private static InspectionManager _instance;
        public static InspectionManager Instance => _instance;

        [Header("Raycast Settings")]
        [SerializeField] private float minDistance = 1f;
        [SerializeField] private float maxDistance = 10f;
        [SerializeField] private float sphereCastRadius = 0.05f;
        [SerializeField] private LayerMask inspectionLayerMask = ~0;

        [Header("References")]
        [SerializeField] private InspectionHUD inspectionHUD;

        private IInspectable _currentTarget;
        private GameObject _currentTargetObject;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
        }

        private void Update()
        {
            UpdateInspection();
        }

        private void UpdateInspection()
        {
            var handsManager = VRHandsManager.Instance;
            if (handsManager == null) return;

            IInspectable foundTarget = null;
            GameObject foundObject = null;

            if (TryGetInspectableFromHand(handsManager.LeftHand, out var leftTarget, out var leftObject))
            {
                foundTarget = leftTarget;
                foundObject = leftObject;
            }
            else if (TryGetInspectableFromHand(handsManager.RightHand, out var rightTarget, out var rightObject))
            {
                foundTarget = rightTarget;
                foundObject = rightObject;
            }

            if (foundTarget != _currentTarget || foundObject != _currentTargetObject)
            {
                _currentTarget = foundTarget;
                _currentTargetObject = foundObject;
                UpdateHUD();
            }
        }

        private bool TryGetInspectableFromHand(Transform hand, out IInspectable inspectable, out GameObject targetObject)
        {
            inspectable = null;
            targetObject = null;

            if (hand == null) return false;

            if (!Physics.SphereCast(hand.position, sphereCastRadius, hand.forward, out RaycastHit hit, maxDistance, inspectionLayerMask))
                return false;

            float distance = hit.distance;
            if (distance < minDistance || distance > maxDistance)
                return false;

            targetObject = hit.collider.gameObject;
            inspectable = GetInspectableFromHit(hit);

            return inspectable != null;
        }

        private IInspectable GetInspectableFromHit(RaycastHit hit)
        {
            var inspectable = hit.collider.GetComponent<IInspectable>();
            if (inspectable != null) return inspectable;

            inspectable = hit.collider.GetComponentInParent<IInspectable>();
            return inspectable;
        }

        private void UpdateHUD()
        {
            if (inspectionHUD == null) return;

            if (_currentTarget != null)
            {
                string name = _currentTarget.GetInspectionName();
                string details = _currentTarget.GetInspectionDetails();
                inspectionHUD.Show(name, details);
            }
            else
            {
                inspectionHUD.Hide();
            }
        }
    }
}

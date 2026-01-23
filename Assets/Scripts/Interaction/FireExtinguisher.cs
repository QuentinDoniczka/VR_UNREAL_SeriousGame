using Core.Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Interaction
{
    public class FireExtinguisher : GrabbableObject
    {
        [Header("Fire Extinguisher Settings")]
        [SerializeField] private ExtinguisherType extinguisherType = ExtinguisherType.CO2;

        [Header("Two-Handed Grip")]
        [SerializeField] private Transform secondaryGripPoint;
        [SerializeField] private GameObject secondaryGripHandle;
        [SerializeField] private float secondaryGripDistance = 0.5f;

        [Header("Safety Pin")]
        [SerializeField] private bool safetyPinEnabled = false;

        [Header("Spray Settings")]
        [SerializeField] private float sprayRange = 5f;
        [SerializeField] private float sprayConeAngle = 30f;
        [SerializeField] private LayerMask sprayLayerMask = ~0;

        [Header("Spray Cone")]
        [SerializeField] private GameObject sprayCone;
        [SerializeField] private bool showSprayConeVisual = true;

        private Transform secondaryHand;
        private bool isSecondaryGrabbed;
        private bool isSpraying;

        private const int MaxSprayHits = 32;
        private readonly Collider[] sprayHitsBuffer = new Collider[MaxSprayHits];

        protected override void Awake()
        {
            base.Awake();

            if (secondaryGripHandle != null)
            {
                secondaryGripHandle.SetActive(false);
            }

            if (sprayCone == null)
            {
                Debug.LogWarning("[FireExtinguisher] Spray Cone is not assigned. Spray detection will not work.", this);
            }
            else
            {
                sprayCone.SetActive(false);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            var input = InputManager.Instance;
            if (input == null) return;

            if (input.GrabLeft != null)
            {
                input.GrabLeft.performed += OnSecondaryGrabPerformed;
                input.GrabLeft.canceled += OnSecondaryGrabCanceled;
            }
            if (input.GrabRight != null)
            {
                input.GrabRight.performed += OnSecondaryGrabPerformed;
                input.GrabRight.canceled += OnSecondaryGrabCanceled;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            var input = InputManager.Instance;
            if (input == null) return;

            if (input.GrabLeft != null)
            {
                input.GrabLeft.performed -= OnSecondaryGrabPerformed;
                input.GrabLeft.canceled -= OnSecondaryGrabCanceled;
            }
            if (input.GrabRight != null)
            {
                input.GrabRight.performed -= OnSecondaryGrabPerformed;
                input.GrabRight.canceled -= OnSecondaryGrabCanceled;
            }
        }

        protected override void Update()
        {
            base.Update();

            if (isSecondaryGrabbed && secondaryHand != null && secondaryGripHandle != null)
            {
                FollowHandleToSecondaryHand();
            }

            if (isSpraying)
            {
                DetectObjectsInCone();
            }

            UpdateTriggerState();
        }

        private void OnSecondaryGrabPerformed(InputAction.CallbackContext context)
        {
            if (!isGrabbed) return;

            Transform triggeringHand = GetHandFromContext(context);
            if (triggeringHand == null) return;
            if (triggeringHand == handTransform) return;

            Transform freeHand = GetFreeHand();
            if (freeHand == null || freeHand != triggeringHand) return;

            secondaryHand = freeHand;
            isSecondaryGrabbed = true;

            if (secondaryGripHandle != null)
            {
                secondaryGripHandle.SetActive(true);
            }
        }

        private void OnSecondaryGrabCanceled(InputAction.CallbackContext context)
        {
            if (!isSecondaryGrabbed) return;

            Transform releasingHand = GetHandFromContext(context);
            if (releasingHand == secondaryHand)
            {
                secondaryHand = null;
                isSecondaryGrabbed = false;

                if (secondaryGripHandle != null)
                {
                    secondaryGripHandle.SetActive(false);
                }
            }
        }

        private Transform GetFreeHand()
        {
            if (handTransform == GetLeftHand())
                return GetRightHand();
            if (handTransform == GetRightHand())
                return GetLeftHand();

            return null;
        }

        private Transform GetHandFromContext(InputAction.CallbackContext context)
        {
            var input = InputManager.Instance;
            if (input == null) return null;

            if (context.action == input.GrabLeft)
                return GetLeftHand();
            if (context.action == input.GrabRight)
                return GetRightHand();

            return null;
        }

        private void UpdateTriggerState()
        {
            if (!isGrabbed || !isSecondaryGrabbed)
            {
                StopSpray();
                return;
            }

            if (safetyPinEnabled)
            {
                StopSpray();
                return;
            }

            bool triggerPressed = false;

            if (secondaryHand != null)
            {
                var input = InputManager.Instance;
                if (input == null) return;

                if (secondaryHand == GetLeftHand() && input.TriggerLeft != null)
                    triggerPressed = input.TriggerLeft.ReadValue<float>() > 0.5f;
                else if (secondaryHand == GetRightHand() && input.TriggerRight != null)
                    triggerPressed = input.TriggerRight.ReadValue<float>() > 0.5f;
            }

            if (triggerPressed && !isSpraying)
                StartSpray();
            else if (!triggerPressed && isSpraying)
                StopSpray();
        }

        private void FollowHandleToSecondaryHand()
        {
            if (secondaryGripHandle == null || secondaryHand == null) return;

            secondaryGripHandle.transform.position = Vector3.Lerp(
                secondaryGripHandle.transform.position,
                secondaryHand.position,
                Time.deltaTime * positionFollowSpeed
            );

            secondaryGripHandle.transform.rotation = Quaternion.Slerp(
                secondaryGripHandle.transform.rotation,
                secondaryHand.rotation,
                Time.deltaTime * rotationFollowSpeed
            );
        }

        protected override void Release()
        {
            base.Release();
            secondaryHand = null;
            isSecondaryGrabbed = false;

            if (secondaryGripHandle != null)
            {
                secondaryGripHandle.SetActive(false);
            }

            StopSpray();
        }

        private void StartSpray()
        {
            isSpraying = true;
            if (sprayCone != null && showSprayConeVisual)
                sprayCone.SetActive(true);
        }

        private void StopSpray()
        {
            if (!isSpraying) return;

            isSpraying = false;
            if (sprayCone != null)
                sprayCone.SetActive(false);
        }

        private void DetectObjectsInCone()
        {
            if (secondaryGripHandle == null) return;

            Vector3 origin = secondaryGripHandle.transform.position;
            Vector3 direction = secondaryGripHandle.transform.forward;

            int hitCount = Physics.OverlapSphereNonAlloc(origin, sprayRange, sprayHitsBuffer, sprayLayerMask);

            for (int i = 0; i < hitCount; i++)
            {
                Collider hit = sprayHitsBuffer[i];
                Vector3 dirToTarget = hit.transform.position - origin;
                float angleToTarget = Vector3.Angle(direction, dirToTarget);

                if (angleToTarget > sprayConeAngle) continue;

                // TODO: Apply extinguisher effect to hit.gameObject
            }
        }

        public ExtinguisherType Type => extinguisherType;
    }
}

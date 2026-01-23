using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;

namespace Interaction
{
    public enum ExtinguisherType
    {
        CO2,
        Foam,
        Water
    }

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

        private InputAction triggerLeftAction;
        private InputAction triggerRightAction;

        private const int MaxSprayHits = 32;
        private readonly Collider[] sprayHitsBuffer = new Collider[MaxSprayHits];

        protected override void Awake()
        {
            base.Awake();

            triggerLeftAction = inputActions.VRMenu.Get().FindAction("Trigger Left");
            triggerRightAction = inputActions.VRMenu.Get().FindAction("Trigger Right");

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
            grabLeftAction.performed += OnSecondaryGrabPerformed;
            grabRightAction.performed += OnSecondaryGrabPerformed;
            grabLeftAction.canceled += OnSecondaryGrabCanceled;
            grabRightAction.canceled += OnSecondaryGrabCanceled;

            triggerLeftAction.performed += OnTriggerLeftPerformed;
            triggerLeftAction.canceled += OnTriggerLeftCanceled;
            triggerRightAction.performed += OnTriggerRightPerformed;
            triggerRightAction.canceled += OnTriggerRightCanceled;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            grabLeftAction.performed -= OnSecondaryGrabPerformed;
            grabRightAction.performed -= OnSecondaryGrabPerformed;
            grabLeftAction.canceled -= OnSecondaryGrabCanceled;
            grabRightAction.canceled -= OnSecondaryGrabCanceled;

            triggerLeftAction.performed -= OnTriggerLeftPerformed;
            triggerLeftAction.canceled -= OnTriggerLeftCanceled;
            triggerRightAction.performed -= OnTriggerRightPerformed;
            triggerRightAction.canceled -= OnTriggerRightCanceled;
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
            if (context.action == grabLeftAction)
                return GetLeftHand();
            if (context.action == grabRightAction)
                return GetRightHand();

            return null;
        }

        private void OnTriggerLeftPerformed(InputAction.CallbackContext context)
        {
            DevLog.Log("Left trigger pressed");
        }

        private void OnTriggerLeftCanceled(InputAction.CallbackContext context)
        {
            DevLog.Log("Left trigger released");
        }

        private void OnTriggerRightPerformed(InputAction.CallbackContext context)
        {
            DevLog.Log("Right trigger pressed");
        }

        private void OnTriggerRightCanceled(InputAction.CallbackContext context)
        {
            DevLog.Log("Right trigger released");
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
                float leftValue = triggerLeftAction.ReadValue<float>();
                float rightValue = triggerRightAction.ReadValue<float>();

                if (secondaryHand == GetLeftHand())
                    triggerPressed = leftValue > 0.5f;
                else if (secondaryHand == GetRightHand())
                    triggerPressed = rightValue > 0.5f;
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

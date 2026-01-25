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

        [Header("Spray Cone")]
        [SerializeField] private GameObject sprayCone;
        [SerializeField] private bool showSprayConeVisual = true;
        [SerializeField] private float extinguishingPower = 0.5f;

        [Header("Input")]
        [SerializeField] private float triggerThreshold = 0.5f;

        private Transform secondaryHand;
        private bool isSecondaryGrabbed;
        private bool isSpraying;

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

        protected override void Update()
        {
            base.Update();

            if (isSecondaryGrabbed && secondaryHand != null && secondaryGripHandle != null)
            {
                FollowHandleToSecondaryHand();
            }

            UpdateTriggerState();
        }

        protected override void OnGrabLeftPerformed(InputAction.CallbackContext context)
        {
            if (TrySecondaryGrab(GetLeftHand())) return;
            base.OnGrabLeftPerformed(context);
        }

        protected override void OnGrabLeftCanceled(InputAction.CallbackContext context)
        {
            if (TrySecondaryRelease(GetLeftHand())) return;
            base.OnGrabLeftCanceled(context);
        }

        protected override void OnGrabRightPerformed(InputAction.CallbackContext context)
        {
            if (TrySecondaryGrab(GetRightHand())) return;
            base.OnGrabRightPerformed(context);
        }

        protected override void OnGrabRightCanceled(InputAction.CallbackContext context)
        {
            if (TrySecondaryRelease(GetRightHand())) return;
            base.OnGrabRightCanceled(context);
        }

        private bool TrySecondaryGrab(Transform hand)
        {
            if (!isGrabbed) return false;
            if (hand == null || hand == handTransform) return false;

            secondaryHand = hand;
            isSecondaryGrabbed = true;

            if (secondaryGripHandle != null)
            {
                secondaryGripHandle.SetActive(true);
            }
            return true;
        }

        private bool TrySecondaryRelease(Transform hand)
        {
            if (!isSecondaryGrabbed) return false;
            if (hand != secondaryHand) return false;

            secondaryHand = null;
            isSecondaryGrabbed = false;

            if (secondaryGripHandle != null)
            {
                secondaryGripHandle.SetActive(false);
            }
            return true;
        }

        private void UpdateTriggerState()
        {
            if (!CanSpray())
            {
                StopSpray();
                return;
            }

            bool triggerPressed = IsSecondaryTriggerPressed();

            if (triggerPressed && !isSpraying)
                StartSpray();
            else if (!triggerPressed && isSpraying)
                StopSpray();
        }

        private bool CanSpray()
        {
            return isGrabbed && isSecondaryGrabbed && !safetyPinEnabled;
        }

        private bool IsSecondaryTriggerPressed()
        {
            if (secondaryHand == null) return false;

            var input = InputManager.Instance;
            if (input == null) return false;

            if (secondaryHand == GetLeftHand() && input.TriggerLeft != null)
                return input.TriggerLeft.ReadValue<float>() > triggerThreshold;

            if (secondaryHand == GetRightHand() && input.TriggerRight != null)
                return input.TriggerRight.ReadValue<float>() > triggerThreshold;

            return false;
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

        public ExtinguisherType Type => extinguisherType;
        public float ExtinguishingPower => extinguishingPower;
    }
}

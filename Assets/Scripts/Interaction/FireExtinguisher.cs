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

        protected override void OnEnable()
        {
            base.OnEnable();
            SubscribeToGrabInputs(true);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            SubscribeToGrabInputs(false);
        }

        private void SubscribeToGrabInputs(bool subscribe)
        {
            var input = InputManager.Instance;
            if (input == null) return;

            if (input.GrabLeft != null)
            {
                if (subscribe)
                {
                    input.GrabLeft.performed += OnSecondaryGrabPerformed;
                    input.GrabLeft.canceled += OnSecondaryGrabCanceled;
                }
                else
                {
                    input.GrabLeft.performed -= OnSecondaryGrabPerformed;
                    input.GrabLeft.canceled -= OnSecondaryGrabCanceled;
                }
            }

            if (input.GrabRight != null)
            {
                if (subscribe)
                {
                    input.GrabRight.performed += OnSecondaryGrabPerformed;
                    input.GrabRight.canceled += OnSecondaryGrabCanceled;
                }
                else
                {
                    input.GrabRight.performed -= OnSecondaryGrabPerformed;
                    input.GrabRight.canceled -= OnSecondaryGrabCanceled;
                }
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
    }
}

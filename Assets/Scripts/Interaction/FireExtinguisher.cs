using UnityEngine;
using UnityEngine.InputSystem;

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
        [SerializeField] private Transform nozzle;

        [Header("Two-Handed Grip")]
        [SerializeField] private Transform secondaryGripPoint;
        [SerializeField] private GameObject secondaryGripHandle;
        [SerializeField] private float secondaryGripDistance = 0.5f;

        private InputAction useAction;
        private Transform secondaryHand;
        private bool isSecondaryGrabbed;

        protected override void Awake()
        {
            base.Awake();
            useAction = inputActions.VRMenu.Use;

            if (secondaryGripHandle != null)
            {
                secondaryGripHandle.SetActive(false);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            useAction.performed += OnUsePerformed;
            grabLeftAction.performed += OnSecondaryGrabPerformed;
            grabRightAction.performed += OnSecondaryGrabPerformed;
            grabLeftAction.canceled += OnSecondaryGrabCanceled;
            grabRightAction.canceled += OnSecondaryGrabCanceled;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            useAction.performed -= OnUsePerformed;
            grabLeftAction.performed -= OnSecondaryGrabPerformed;
            grabRightAction.performed -= OnSecondaryGrabPerformed;
            grabLeftAction.canceled -= OnSecondaryGrabCanceled;
            grabRightAction.canceled -= OnSecondaryGrabCanceled;
        }

        private void OnUsePerformed(InputAction.CallbackContext context)
        {
            if (isGrabbed)
            {
                Use();
            }
        }

        private void Use()
        {
        }

        protected override void Update()
        {
            base.Update();

            if (isSecondaryGrabbed && secondaryHand != null && secondaryGripHandle != null)
            {
                FollowHandleToSecondaryHand();
            }
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
            GameObject leftHandGO = GameObject.FindGameObjectWithTag("LeftHand");
            GameObject rightHandGO = GameObject.FindGameObjectWithTag("RightHand");

            Transform leftHand = leftHandGO != null ? leftHandGO.transform : null;
            Transform rightHand = rightHandGO != null ? rightHandGO.transform : null;

            if (handTransform == leftHand)
                return rightHand;
            else if (handTransform == rightHand)
                return leftHand;

            return null;
        }

        private Transform GetHandFromContext(InputAction.CallbackContext context)
        {
            GameObject leftHandGO = GameObject.FindGameObjectWithTag("LeftHand");
            GameObject rightHandGO = GameObject.FindGameObjectWithTag("RightHand");

            Transform leftHand = leftHandGO != null ? leftHandGO.transform : null;
            Transform rightHand = rightHandGO != null ? rightHandGO.transform : null;

            if (context.action == grabLeftAction)
                return leftHand;
            else if (context.action == grabRightAction)
                return rightHand;

            return null;
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
        }

        public ExtinguisherType Type => extinguisherType;
    }
}

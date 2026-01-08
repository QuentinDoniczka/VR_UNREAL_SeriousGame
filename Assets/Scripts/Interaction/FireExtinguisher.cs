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
        [SerializeField] private float secondaryGripDistance = 0.5f;

        private VRMenuInputActions inputActions;
        private InputAction useAction;
        private InputAction grabLeftAction;
        private InputAction grabRightAction;

        private Transform secondaryHand;
        private bool isSecondaryGrabbed;

        protected override void Awake()
        {
            base.Awake();
            inputActions = new VRMenuInputActions();
            useAction = inputActions.VRMenu.Use;
            grabLeftAction = inputActions.VRMenu.GrabLeft;
            grabRightAction = inputActions.VRMenu.GrabRight;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            inputActions.Enable();
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
            inputActions.Disable();
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

            if (isSecondaryGrabbed && secondaryHand != null && secondaryGripPoint != null)
            {
                FollowSecondaryHand();
            }
        }

        private void OnSecondaryGrabPerformed(InputAction.CallbackContext context)
        {
            if (!isGrabbed) return;

            Transform freeHand = GetFreeHand();
            if (freeHand == null) return;

            if (secondaryGripPoint == null)
            {
                secondaryHand = freeHand;
                isSecondaryGrabbed = true;
                return;
            }

            float distance = Vector3.Distance(freeHand.position, secondaryGripPoint.position);
            if (distance <= secondaryGripDistance)
            {
                secondaryHand = freeHand;
                isSecondaryGrabbed = true;
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

        private void FollowSecondaryHand()
        {
            if (secondaryGripPoint == null) return;

            Vector3 directionToGrip = secondaryGripPoint.position - secondaryHand.position;
            transform.position -= directionToGrip;
        }

        protected override void Release()
        {
            base.Release();
            secondaryHand = null;
            isSecondaryGrabbed = false;
        }

        public ExtinguisherType Type => extinguisherType;
    }
}

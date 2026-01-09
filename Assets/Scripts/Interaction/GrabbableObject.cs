using UnityEngine;
using UnityEngine.InputSystem;

namespace Interaction
{
    [RequireComponent(typeof(Rigidbody))]
    public class GrabbableObject : MonoBehaviour
    {
        [Header("Grab Settings")]
        [SerializeField] protected float grabDistance = 100.5f;
        [SerializeField] protected LayerMask grabLayerMask = ~0;

        [Header("Hold Position & Rotation")]
        [SerializeField] protected Vector3 holdPositionOffset = new Vector3(0f, 0f, 0.3f);
        [SerializeField] protected Vector3 holdRotationOffset = Vector3.zero;

        [Header("Follow Settings")]
        [SerializeField] protected float positionFollowSpeed = 20f;
        [SerializeField] protected float rotationFollowSpeed = 20f;

        protected bool isGrabbed;
        protected Transform handTransform;
        protected Rigidbody rb;

        protected VRMenuInputActions inputActions;
        protected InputAction grabLeftAction;
        protected InputAction grabRightAction;

        private const string LeftHandTag = "LeftHand";
        private const string RightHandTag = "RightHand";

        private Transform leftHand;
        private Transform rightHand;

        protected virtual void Awake()
        {
            rb = GetComponent<Rigidbody>();
            inputActions = new VRMenuInputActions();
            grabLeftAction = inputActions.VRMenu.GrabLeft;
            grabRightAction = inputActions.VRMenu.GrabRight;
        }

        private Transform GetHand(string handTag, ref Transform cachedHand)
        {
            if (cachedHand == null)
            {
                GameObject handGO = GameObject.FindGameObjectWithTag(handTag);
                if (handGO != null)
                {
                    cachedHand = handGO.transform;
                }
            }
            return cachedHand;
        }

        private Transform GetLeftHand() => GetHand(LeftHandTag, ref leftHand);
        private Transform GetRightHand() => GetHand(RightHandTag, ref rightHand);

        protected virtual void OnEnable()
        {
            inputActions.Enable();
            grabLeftAction.performed += OnGrabLeftPerformed;
            grabLeftAction.canceled += OnGrabLeftCanceled;
            grabRightAction.performed += OnGrabRightPerformed;
            grabRightAction.canceled += OnGrabRightCanceled;
        }

        protected virtual void OnDisable()
        {
            grabLeftAction.performed -= OnGrabLeftPerformed;
            grabLeftAction.canceled -= OnGrabLeftCanceled;
            grabRightAction.performed -= OnGrabRightPerformed;
            grabRightAction.canceled -= OnGrabRightCanceled;
            inputActions.Disable();

            leftHand = null;
            rightHand = null;
        }

        protected virtual void Update()
        {
            if (isGrabbed && handTransform != null)
            {
                FollowHand();
            }
        }

        private void OnGrabLeftPerformed(InputAction.CallbackContext context)
        {
            Transform hand = GetLeftHand();
            if (hand == null) return;

            if (isGrabbed) return;

            if (IsHandInRange(hand))
            {
                Grab(hand);
            }
        }

        private void OnGrabLeftCanceled(InputAction.CallbackContext context)
        {
            Transform hand = GetLeftHand();
            if (isGrabbed && handTransform == hand)
            {
                Release();
            }
        }

        private void OnGrabRightPerformed(InputAction.CallbackContext context)
        {
            Transform hand = GetRightHand();
            if (hand == null) return;

            if (isGrabbed) return;

            if (IsHandInRange(hand))
            {
                Grab(hand);
            }
        }

        private void OnGrabRightCanceled(InputAction.CallbackContext context)
        {
            Transform hand = GetRightHand();
            if (isGrabbed && handTransform == hand)
            {
                Release();
            }
        }

        protected virtual void Grab(Transform hand)
        {
            isGrabbed = true;
            handTransform = hand;
            rb.isKinematic = true;
        }

        protected virtual void Release()
        {
            isGrabbed = false;
            handTransform = null;
            rb.isKinematic = false;
        }

        protected virtual void FollowHand()
        {
            Vector3 posOffset = handTransform.TransformDirection(holdPositionOffset);
            Vector3 targetPosition = handTransform.position + posOffset;
            Quaternion targetRotation = handTransform.rotation * Quaternion.Euler(holdRotationOffset);

            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * positionFollowSpeed);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationFollowSpeed);
        }

        protected virtual bool IsHandInRange(Transform hand)
        {
            if (Physics.Raycast(hand.position, hand.forward, out RaycastHit hit, grabDistance, grabLayerMask))
            {
                return hit.collider.gameObject == gameObject || hit.collider.transform.IsChildOf(transform);
            }
            return false;
        }

        public bool IsGrabbed => isGrabbed;
    }
}

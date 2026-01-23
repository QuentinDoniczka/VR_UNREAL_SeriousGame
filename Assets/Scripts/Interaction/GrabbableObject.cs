using Core.Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Interaction
{
    [RequireComponent(typeof(Rigidbody))]
    public class GrabbableObject : MonoBehaviour
    {
        [Header("Grab Settings")]
        [SerializeField] protected float grabDistance = 100.5f;
        [SerializeField] protected float sphereCastRadius = 0.05f;
        [SerializeField] protected LayerMask grabLayerMask = ~0;

        [Header("Hold Position & Rotation")]
        [SerializeField] protected Vector3 holdPositionOffset = new Vector3(0f, 0f, 0.3f);
        [SerializeField] protected Vector3 holdRotationOffset = Vector3.zero;

        [Header("Follow Settings")]
        [SerializeField] protected float positionFollowSpeed = 20f;
        [SerializeField] protected float rotationFollowSpeed = 20f;

        [Header("Highlight Settings")]
        [SerializeField] private bool enableHighlight = true;
        [SerializeField] private float hoverStabilityTime = 0.1f;

        protected bool isGrabbed;
        protected Transform handTransform;
        protected Rigidbody rb;
        protected InteractableHighlight highlight;

        private const string LeftHandTag = "LeftHand";
        private const string RightHandTag = "RightHand";

        private Transform leftHand;
        private Transform rightHand;
        private bool isLeftHandHovering;
        private bool isRightHandHovering;
        private bool isHighlightActive;
        private float hoverTimer;

        protected virtual void Awake()
        {
            rb = GetComponent<Rigidbody>();
            SetupHighlight();
        }

        private void SetupHighlight()
        {
            if (enableHighlight)
            {
                highlight = GetComponent<InteractableHighlight>();
                if (highlight == null)
                {
                    highlight = gameObject.AddComponent<InteractableHighlight>();
                }
            }
            else
            {
                highlight = GetComponent<InteractableHighlight>();
                if (highlight != null)
                {
                    Destroy(highlight);
                    highlight = null;
                }
            }
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

        protected Transform GetLeftHand() => GetHand(LeftHandTag, ref leftHand);
        protected Transform GetRightHand() => GetHand(RightHandTag, ref rightHand);

        protected virtual void OnEnable()
        {
            var input = InputManager.Instance;
            if (input == null) return;

            if (input.GrabLeft != null)
            {
                input.GrabLeft.performed += OnGrabLeftPerformed;
                input.GrabLeft.canceled += OnGrabLeftCanceled;
            }
            if (input.GrabRight != null)
            {
                input.GrabRight.performed += OnGrabRightPerformed;
                input.GrabRight.canceled += OnGrabRightCanceled;
            }
        }

        protected virtual void OnDisable()
        {
            var input = InputManager.Instance;
            if (input != null)
            {
                if (input.GrabLeft != null)
                {
                    input.GrabLeft.performed -= OnGrabLeftPerformed;
                    input.GrabLeft.canceled -= OnGrabLeftCanceled;
                }
                if (input.GrabRight != null)
                {
                    input.GrabRight.performed -= OnGrabRightPerformed;
                    input.GrabRight.canceled -= OnGrabRightCanceled;
                }
            }

            leftHand = null;
            rightHand = null;
        }

        protected virtual void Update()
        {
            if (isGrabbed && handTransform != null)
            {
                FollowHand();
            }
            else
            {
                UpdateHoverState();
            }
        }

        private void UpdateHoverState()
        {
            if (highlight == null) return;

            Transform leftHandTransform = GetLeftHand();
            Transform rightHandTransform = GetRightHand();

            isLeftHandHovering = leftHandTransform != null && IsHandInRange(leftHandTransform);
            isRightHandHovering = rightHandTransform != null && IsHandInRange(rightHandTransform);

            bool shouldBeHighlighted = isLeftHandHovering || isRightHandHovering;

            if (shouldBeHighlighted && !isHighlightActive)
            {
                hoverTimer += Time.deltaTime;
                if (hoverTimer >= hoverStabilityTime)
                {
                    isHighlightActive = true;
                    highlight.SetHighlighted(true);
                }
            }
            else if (!shouldBeHighlighted && isHighlightActive)
            {
                hoverTimer = 0f;
                isHighlightActive = false;
                highlight.SetHighlighted(false);
            }
            else if (shouldBeHighlighted && isHighlightActive)
            {
                hoverTimer = hoverStabilityTime;
            }
            else
            {
                hoverTimer = Mathf.Max(0f, hoverTimer - Time.deltaTime * 2f);
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

            hoverTimer = 0f;
            isHighlightActive = false;
            if (highlight != null)
            {
                highlight.SetHighlighted(false);
            }
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
            if (Physics.SphereCast(hand.position, sphereCastRadius, hand.forward, out RaycastHit hit, grabDistance, grabLayerMask))
            {
                return hit.collider.gameObject == gameObject || hit.collider.transform.IsChildOf(transform);
            }
            return false;
        }

        public bool IsGrabbed => isGrabbed;
    }
}

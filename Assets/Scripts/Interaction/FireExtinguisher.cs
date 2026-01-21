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
        [SerializeField] private Transform nozzle;

        [Header("Two-Handed Grip")]
        [SerializeField] private Transform secondaryGripPoint;
        [SerializeField] private GameObject secondaryGripHandle;
        [SerializeField] private float secondaryGripDistance = 0.5f;

        [Header("Safety Pin")]
        [SerializeField] private bool safetyPinEnabled = true;

        [Header("Spray Settings")]
        [SerializeField] private float sprayRange = 5f;
        [SerializeField] private float sprayConeAngle = 30f;
        [SerializeField] private LayerMask sprayLayerMask = ~0;

        [Header("Debug Hitbox Visual")]
        [SerializeField] private bool showDebugHitbox = true;
        [SerializeField] private Material debugHitboxMaterial;

        private Transform secondaryHand;
        private bool isSecondaryGrabbed;
        private bool isSpraying;
        private GameObject debugHitboxVisual;

        private InputAction triggerLeftAction;
        private InputAction triggerRightAction;
        private Material createdDebugMaterial;

        private Transform cachedLeftHand;
        private Transform cachedRightHand;
        private bool handsCached;

        protected override void Awake()
        {
            base.Awake();

            triggerLeftAction = inputActions.VRMenu.Get().FindAction("Trigger Left");
            triggerRightAction = inputActions.VRMenu.Get().FindAction("Trigger Right");

            if (secondaryGripHandle != null)
            {
                secondaryGripHandle.SetActive(false);
            }

            if (showDebugHitbox)
            {
                CreateDebugHitboxVisual();
            }
        }

        private void EnsureHandsCached()
        {
            if (handsCached) return;

            GameObject leftHandGO = GameObject.FindGameObjectWithTag("LeftHand");
            GameObject rightHandGO = GameObject.FindGameObjectWithTag("RightHand");

            cachedLeftHand = leftHandGO != null ? leftHandGO.transform : null;
            cachedRightHand = rightHandGO != null ? rightHandGO.transform : null;

            handsCached = cachedLeftHand != null && cachedRightHand != null;
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

        private void OnDestroy()
        {
            if (createdDebugMaterial != null)
            {
                Destroy(createdDebugMaterial);
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
                UpdateDebugHitboxVisual();
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
            EnsureHandsCached();

            if (handTransform == cachedLeftHand)
                return cachedRightHand;
            if (handTransform == cachedRightHand)
                return cachedLeftHand;

            return null;
        }

        private Transform GetHandFromContext(InputAction.CallbackContext context)
        {
            EnsureHandsCached();

            if (context.action == grabLeftAction)
                return cachedLeftHand;
            if (context.action == grabRightAction)
                return cachedRightHand;

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

            EnsureHandsCached();

            bool triggerPressed = false;

            if (secondaryHand != null)
            {
                float leftValue = triggerLeftAction.ReadValue<float>();
                float rightValue = triggerRightAction.ReadValue<float>();

                if (secondaryHand == cachedLeftHand)
                    triggerPressed = leftValue > 0.5f;
                else if (secondaryHand == cachedRightHand)
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

        private void CreateDebugHitboxVisual()
        {
            debugHitboxVisual = new GameObject("DebugHitboxCone");
            debugHitboxVisual.transform.SetParent(transform);
            debugHitboxVisual.transform.localPosition = Vector3.zero;
            debugHitboxVisual.transform.localRotation = Quaternion.identity;

            MeshFilter meshFilter = debugHitboxVisual.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = debugHitboxVisual.AddComponent<MeshRenderer>();

            meshFilter.mesh = MeshGenerator.CreateCone(sprayRange, sprayConeAngle);

            if (debugHitboxMaterial != null)
            {
                meshRenderer.material = debugHitboxMaterial;
            }
            else
            {
                Shader standardShader = Shader.Find("Standard");
                if (standardShader == null)
                {
                    Debug.LogError("[FireExtinguisher] Standard shader not found. Assign a debugHitboxMaterial in the inspector.", this);
                    return;
                }

                createdDebugMaterial = new Material(standardShader);
                createdDebugMaterial.color = new Color(1f, 1f, 1f, 0.3f);
                createdDebugMaterial.SetFloat("_Mode", 3);
                createdDebugMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                createdDebugMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                createdDebugMaterial.SetInt("_ZWrite", 0);
                createdDebugMaterial.DisableKeyword("_ALPHATEST_ON");
                createdDebugMaterial.EnableKeyword("_ALPHABLEND_ON");
                createdDebugMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                createdDebugMaterial.renderQueue = 3000;
                meshRenderer.material = createdDebugMaterial;
            }

            debugHitboxVisual.SetActive(false);
        }

        private void StartSpray()
        {
            isSpraying = true;
            if (debugHitboxVisual != null)
                debugHitboxVisual.SetActive(true);
        }

        private void StopSpray()
        {
            if (!isSpraying) return;

            isSpraying = false;
            if (debugHitboxVisual != null)
                debugHitboxVisual.SetActive(false);
        }

        private void UpdateDebugHitboxVisual()
        {
            if (debugHitboxVisual == null || secondaryGripHandle == null) return;

            debugHitboxVisual.transform.position = secondaryGripHandle.transform.position;
            debugHitboxVisual.transform.rotation = secondaryGripHandle.transform.rotation;
        }

        private void DetectObjectsInCone()
        {
            if (secondaryGripHandle == null) return;

            Vector3 origin = secondaryGripHandle.transform.position;
            Vector3 direction = secondaryGripHandle.transform.forward;

            Collider[] hits = Physics.OverlapSphere(origin, sprayRange, sprayLayerMask);

            foreach (Collider hit in hits)
            {
                Vector3 dirToTarget = hit.transform.position - origin;
                float angleToTarget = Vector3.Angle(direction, dirToTarget);

                if (angleToTarget > sprayConeAngle) continue;

                // TODO: Apply extinguisher effect to hit.gameObject
            }
        }

        public ExtinguisherType Type => extinguisherType;
    }
}

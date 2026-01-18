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

        [Header("Safety Pin")]
        [SerializeField] private bool safetyPinEnabled = true;

        [Header("Spray Settings")]
        [SerializeField] private float sprayRange = 5f;
        [SerializeField] private float sprayConeAngle = 30f;
        [SerializeField] private Material sprayConeMaterial;
        [SerializeField] private LayerMask sprayLayerMask = ~0;

        private Transform secondaryHand;
        private bool isSecondaryGrabbed;
        private bool isSpraying;
        private GameObject sprayVisualCone;

        private InputAction triggerLeftAction;
        private InputAction triggerRightAction;

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

            CreateSprayVisual();
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
            triggerRightAction.performed += OnTriggerRightPerformed;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            grabLeftAction.performed -= OnSecondaryGrabPerformed;
            grabRightAction.performed -= OnSecondaryGrabPerformed;
            grabLeftAction.canceled -= OnSecondaryGrabCanceled;
            grabRightAction.canceled -= OnSecondaryGrabCanceled;

            triggerLeftAction.performed -= OnTriggerLeftPerformed;
            triggerRightAction.performed -= OnTriggerRightPerformed;
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
                UpdateSprayVisual();
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
            Debug.Log("LEFT TRIGGER PERFORMED");
        }

        private void OnTriggerRightPerformed(InputAction.CallbackContext context)
        {
            Debug.Log("RIGHT TRIGGER PERFORMED");
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
                {
                    triggerPressed = leftValue > 0.5f;
                    if (leftValue > 0.1f)
                        Debug.Log($"SECONDARY HAND (LEFT) TRIGGER VALUE: {leftValue}");
                }
                else if (secondaryHand == cachedRightHand)
                {
                    triggerPressed = rightValue > 0.5f;
                    if (rightValue > 0.1f)
                        Debug.Log($"SECONDARY HAND (RIGHT) TRIGGER VALUE: {rightValue}");
                }
            }

            if (triggerPressed && !isSpraying)
            {
                Debug.Log("STARTING SPRAY FROM SECONDARY HAND!");
                StartSpray();
            }
            else if (!triggerPressed && isSpraying)
            {
                Debug.Log("STOP SPRAY!");
                StopSpray();
            }
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

        private void CreateSprayVisual()
        {
            sprayVisualCone = new GameObject("SprayCone");
            sprayVisualCone.transform.SetParent(transform);
            sprayVisualCone.transform.localPosition = Vector3.zero;
            sprayVisualCone.transform.localRotation = Quaternion.identity;

            MeshFilter meshFilter = sprayVisualCone.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = sprayVisualCone.AddComponent<MeshRenderer>();

            meshFilter.mesh = CreateConeMesh();

            if (sprayConeMaterial != null)
                meshRenderer.material = sprayConeMaterial;
            else
            {
                Material defaultMat = new Material(Shader.Find("Standard"));
                defaultMat.color = new Color(1f, 1f, 1f, 0.3f);
                defaultMat.SetFloat("_Mode", 3);
                defaultMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                defaultMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                defaultMat.SetInt("_ZWrite", 0);
                defaultMat.DisableKeyword("_ALPHATEST_ON");
                defaultMat.EnableKeyword("_ALPHABLEND_ON");
                defaultMat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                defaultMat.renderQueue = 3000;
                meshRenderer.material = defaultMat;
            }

            sprayVisualCone.SetActive(false);
        }

        private Mesh CreateConeMesh()
        {
            Mesh mesh = new Mesh();
            int segments = 20;
            float angle = sprayConeAngle;
            float range = sprayRange;

            Vector3[] vertices = new Vector3[segments + 2];
            int[] triangles = new int[segments * 6];

            vertices[0] = Vector3.zero;

            float radius = Mathf.Tan(angle * Mathf.Deg2Rad) * range;

            for (int i = 0; i <= segments; i++)
            {
                float currentAngle = (i / (float)segments) * Mathf.PI * 2f;
                float x = Mathf.Cos(currentAngle) * radius;
                float y = Mathf.Sin(currentAngle) * radius;
                vertices[i + 1] = new Vector3(x, y, range);
            }

            for (int i = 0; i < segments; i++)
            {
                int triIndex = i * 6;
                triangles[triIndex] = 0;
                triangles[triIndex + 1] = i + 1;
                triangles[triIndex + 2] = i + 2;

                triangles[triIndex + 3] = 0;
                triangles[triIndex + 4] = i + 2;
                triangles[triIndex + 5] = i + 1;
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();

            return mesh;
        }

        private void StartSpray()
        {
            isSpraying = true;
            if (sprayVisualCone != null)
                sprayVisualCone.SetActive(true);

            Debug.Log("=== SPRAY STARTED ===");
        }

        private void StopSpray()
        {
            if (!isSpraying) return;

            isSpraying = false;
            if (sprayVisualCone != null)
                sprayVisualCone.SetActive(false);

            Debug.Log("=== SPRAY STOPPED ===");
        }

        private void UpdateSprayVisual()
        {
            if (sprayVisualCone == null || secondaryHand == null) return;

            sprayVisualCone.transform.position = secondaryHand.position;
            sprayVisualCone.transform.rotation = secondaryHand.rotation;
        }

        private void DetectObjectsInCone()
        {
            if (secondaryHand == null) return;

            Vector3 origin = secondaryHand.position;
            Vector3 direction = secondaryHand.forward;

            Collider[] hits = Physics.OverlapSphere(origin, sprayRange, sprayLayerMask);

            foreach (Collider hit in hits)
            {
                Vector3 dirToTarget = hit.transform.position - origin;
                float angleToTarget = Vector3.Angle(direction, dirToTarget);

                if (angleToTarget <= sprayConeAngle)
                {
                    Debug.Log($"Spraying on: {hit.gameObject.name}");
                }
            }
        }

        public ExtinguisherType Type => extinguisherType;
    }
}

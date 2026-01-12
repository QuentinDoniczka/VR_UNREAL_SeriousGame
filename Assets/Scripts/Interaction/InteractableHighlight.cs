using UnityEngine;
using UnityEngine.Rendering;

namespace Interaction
{
    public class InteractableHighlight : MonoBehaviour
    {
        [Header("Outline Settings")]
        [SerializeField] private Color outlineColor = new Color(0.3f, 0.8f, 1f, 1f);
        [SerializeField] private float outlineWidth = 0.03f;
        [SerializeField] private float emissionIntensity = 5f;
        [SerializeField] [Range(0f, 1f)] private float transparency = 0.5f;

        private Renderer[] renderers;
        private GameObject[] outlineObjects;
        private bool isHighlighted;

        private static readonly int BaseColorProperty = Shader.PropertyToID("_BaseColor");
        private static readonly int EmissionColorProperty = Shader.PropertyToID("_EmissionColor");
        private static readonly int SurfaceProperty = Shader.PropertyToID("_Surface");
        private static readonly int BlendProperty = Shader.PropertyToID("_Blend");
        private static readonly int SrcBlendProperty = Shader.PropertyToID("_SrcBlend");
        private static readonly int DstBlendProperty = Shader.PropertyToID("_DstBlend");
        private static readonly int ZWriteProperty = Shader.PropertyToID("_ZWrite");

        private void Awake()
        {
            CreateOutlineObjects();
        }

        private void CreateOutlineObjects()
        {
            renderers = GetComponentsInChildren<Renderer>();
            outlineObjects = new GameObject[renderers.Length];

            for (int i = 0; i < renderers.Length; i++)
            {
                MeshFilter meshFilter = renderers[i].GetComponent<MeshFilter>();
                if (meshFilter == null || meshFilter.sharedMesh == null) continue;

                GameObject outlineObj = new GameObject($"{renderers[i].name}_Outline");
                outlineObj.transform.SetParent(renderers[i].transform, false);
                outlineObj.transform.localPosition = Vector3.zero;
                outlineObj.transform.localRotation = Quaternion.identity;
                outlineObj.transform.localScale = Vector3.one;
                outlineObj.layer = renderers[i].gameObject.layer;

                MeshFilter outlineMeshFilter = outlineObj.AddComponent<MeshFilter>();
                outlineMeshFilter.sharedMesh = meshFilter.sharedMesh;

                MeshRenderer outlineRenderer = outlineObj.AddComponent<MeshRenderer>();
                Material outlineMaterial = CreateOutlineMaterial();
                outlineRenderer.material = outlineMaterial;
                outlineRenderer.shadowCastingMode = ShadowCastingMode.Off;
                outlineRenderer.receiveShadows = false;

                outlineObjects[i] = outlineObj;
                outlineObj.SetActive(false);
            }
        }

        private Material CreateOutlineMaterial()
        {
            Material material = new Material(Shader.Find("Universal Render Pipeline/Lit"));

            material.SetFloat(SurfaceProperty, 1);
            material.SetFloat(BlendProperty, 0);
            material.SetFloat(SrcBlendProperty, (float)BlendMode.SrcAlpha);
            material.SetFloat(DstBlendProperty, (float)BlendMode.One);
            material.SetFloat(ZWriteProperty, 0);

            material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            material.EnableKeyword("_EMISSION");
            material.renderQueue = (int)RenderQueue.Transparent;

            Color baseColor = outlineColor;
            baseColor.a = transparency;
            material.SetColor(BaseColorProperty, baseColor);

            Color emissionColor = outlineColor * emissionIntensity;
            material.SetColor(EmissionColorProperty, emissionColor);

            return material;
        }

        public void SetHighlighted(bool highlighted)
        {
            if (isHighlighted == highlighted) return;

            isHighlighted = highlighted;

            for (int i = 0; i < outlineObjects.Length; i++)
            {
                if (outlineObjects[i] != null)
                {
                    outlineObjects[i].SetActive(highlighted);
                    if (highlighted)
                    {
                        outlineObjects[i].transform.localScale = Vector3.one * (1f + outlineWidth);
                    }
                }
            }
        }

        private void OnDestroy()
        {
            if (outlineObjects != null)
            {
                foreach (GameObject obj in outlineObjects)
                {
                    if (obj != null)
                    {
                        if (obj.GetComponent<MeshRenderer>() != null)
                        {
                            Destroy(obj.GetComponent<MeshRenderer>().material);
                        }
                        Destroy(obj);
                    }
                }
            }
        }
    }
}

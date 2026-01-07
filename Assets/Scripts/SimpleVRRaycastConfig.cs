using UnityEngine;
using System.Reflection;

/// <summary>
/// Script simple pour configurer la distance du raycast VR
/// Attacher ce script au XR Origin dans votre scène
/// </summary>
public class SimpleVRRaycastConfig : MonoBehaviour
{
    [Header("Configuration Raycast")]
    [Tooltip("Distance maximale du raycast en mètres")]
    [Range(1f, 50f)]
    public float maxRaycastDistance = 10f;

    [Tooltip("Appliquer automatiquement au démarrage")]
    public bool applyOnStart = true;

    void Start()
    {
        if (applyOnStart)
        {
            ApplyConfiguration();
        }
    }

    [ContextMenu("Appliquer la Configuration")]
    public void ApplyConfiguration()
    {
        int configured = 0;

        Component[] allComponents = GetComponentsInChildren<Component>(true);

        foreach (var component in allComponents)
        {
            if (component == null) continue;

            var type = component.GetType();

            if (type.Name.Contains("CurveInteractionCaster") ||
                type.Name.Contains("RaycastCaster"))
            {
                SetFieldValue(component, "m_CastDistance", maxRaycastDistance);
                configured++;
            }

            if (type.Name.Contains("CurveVisual") ||
                type.Name.Contains("LineVisual"))
            {
                SetFieldValue(component, "m_MaxVisualCurveDistance", maxRaycastDistance);
            }
        }

        Debug.Log($"[SimpleVRRaycastConfig] ✓ Configuration appliquée: {maxRaycastDistance}m sur {configured} raycasters");
    }

    void SetFieldValue(Component component, string fieldName, float value)
    {
        var type = component.GetType();
        var field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        if (field != null && field.FieldType == typeof(float))
        {
            field.SetValue(component, value);
        }
    }
}

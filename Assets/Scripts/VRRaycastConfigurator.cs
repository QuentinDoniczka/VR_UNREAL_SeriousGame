using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Casters;

public class VRRaycastConfigurator : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    [Tooltip("Configuration centralisée du raycast")]
    private VRRaycastSettings settings;

    [Header("Quick Config (sans ScriptableObject)")]
    [SerializeField]
    [Tooltip("Si settings est null, utiliser cette valeur")]
    [Range(1f, 50f)]
    private float quickMaxDistance = 10f;

    void Start()
    {
        ApplySettings();
    }

    void ApplySettings()
    {
        float distance = settings != null ? settings.maxRaycastDistance : quickMaxDistance;
        float visual = settings != null ? settings.maxVisualDistance : quickMaxDistance;
        float radius = settings != null ? settings.sphereCastRadius : 0.1f;
        float angle = settings != null ? settings.coneCastAngle : 6f;

        var casters = GetComponentsInChildren<ICurveInteractionCaster>(true);
        int count = 0;

        foreach (var caster in casters)
        {
            var component = caster as Component;
            if (component != null)
            {
                var serializedObject = new UnityEditor.SerializedObject(component);

                var distanceProp = serializedObject.FindProperty("m_CastDistance");
                if (distanceProp != null)
                {
                    distanceProp.floatValue = distance;
                    count++;
                }

                var radiusProp = serializedObject.FindProperty("m_SphereCastRadius");
                if (radiusProp != null)
                    radiusProp.floatValue = radius;

                var angleProp = serializedObject.FindProperty("m_ConeCastAngle");
                if (angleProp != null)
                    angleProp.floatValue = angle;

                serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        Debug.Log($"[VRRaycastConfigurator] Configuration appliquée sur {count} interactors: Distance={distance}m");
    }

#if UNITY_EDITOR
    [ContextMenu("Apply Settings Now")]
    void ApplyNow()
    {
        ApplySettings();
    }
#endif
}

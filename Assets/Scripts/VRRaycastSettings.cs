using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Casters;

[CreateAssetMenu(fileName = "VRRaycastSettings", menuName = "VR/Raycast Settings")]
public class VRRaycastSettings : ScriptableObject
{
    [Header("Raycast Distance")]
    [Tooltip("Distance maximale du raycast en mètres")]
    [Range(1f, 50f)]
    public float maxRaycastDistance = 10f;

    [Header("Visual Settings")]
    [Tooltip("Distance maximale d'affichage du ray visuel")]
    [Range(1f, 50f)]
    public float maxVisualDistance = 10f;

    [Header("Detection Settings")]
    [Tooltip("Rayon de détection (tolérance de visée)")]
    [Range(0.01f, 0.5f)]
    public float sphereCastRadius = 0.1f;

    [Tooltip("Angle du cone de détection")]
    [Range(1f, 45f)]
    public float coneCastAngle = 6f;
}

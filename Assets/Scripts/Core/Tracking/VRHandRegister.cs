using Core.Managers;
using UnityEngine;

namespace Core.Tracking
{
    public enum HandSide
    {
        Left,
        Right
    }

    public class VRHandRegister : MonoBehaviour
    {
        [SerializeField] private HandSide handSide;

        private void Start()
        {
            if (VRHandsManager.Instance == null)
            {
                Debug.LogError("[VRHandRegister] VRHandsManager not found. Ensure GameManager initializes it.", this);
                return;
            }

            if (handSide == HandSide.Left)
                VRHandsManager.Instance.RegisterLeftHand(transform);
            else
                VRHandsManager.Instance.RegisterRightHand(transform);
        }

        private void OnDestroy()
        {
            if (VRHandsManager.Instance == null) return;

            if (handSide == HandSide.Left)
                VRHandsManager.Instance.UnregisterLeftHand(transform);
            else
                VRHandsManager.Instance.UnregisterRightHand(transform);
        }
    }
}

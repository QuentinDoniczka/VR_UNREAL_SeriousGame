using UnityEngine;

namespace Core.Managers
{
    public class VRHandsManager : MonoBehaviour
    {
        private static VRHandsManager _instance;
        public static VRHandsManager Instance => _instance;

        public Transform LeftHand { get; private set; }
        public Transform RightHand { get; private set; }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this);
                return;
            }
            _instance = this;
        }

        public void RegisterLeftHand(Transform hand)
        {
            LeftHand = hand;
        }

        public void RegisterRightHand(Transform hand)
        {
            RightHand = hand;
        }

        public void UnregisterLeftHand(Transform hand)
        {
            if (LeftHand == hand)
                LeftHand = null;
        }

        public void UnregisterRightHand(Transform hand)
        {
            if (RightHand == hand)
                RightHand = null;
        }
    }
}

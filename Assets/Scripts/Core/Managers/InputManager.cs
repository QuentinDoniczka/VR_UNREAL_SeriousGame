using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Managers
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }

        private VRMenuInputActions inputActions;

        public InputAction GrabLeft => inputActions?.VRMenu.GrabLeft;
        public InputAction GrabRight => inputActions?.VRMenu.GrabRight;
        public InputAction TriggerLeft => inputActions?.VRMenu.TriggerLeft;
        public InputAction TriggerRight => inputActions?.VRMenu.TriggerRight;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;

            inputActions = new VRMenuInputActions();
        }

        private void OnEnable()
        {
            inputActions?.Enable();
        }

        private void OnDisable()
        {
            inputActions?.Disable();
        }
    }
}

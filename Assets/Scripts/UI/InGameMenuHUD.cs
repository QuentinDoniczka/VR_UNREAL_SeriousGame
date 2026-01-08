using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace UI
{
    public class InGameMenuHUD : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject menuPanel;
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private Canvas hudCanvas;
        [SerializeField] private ConfirmationDialog confirmationDialog;

        [Header("HUD Settings")]
        [SerializeField] private float distanceFromCamera = 2f;
        [SerializeField] private Vector3 offset = new Vector3(0f, 0f, 0f);
        [SerializeField] private bool pauseGameWhenOpen = true;

        [Header("Input Settings")]
        [SerializeField] private InputActionProperty menuAction;
        [SerializeField] private bool enableKeyboardInput = true;
        [SerializeField] private Key keyboardToggleKey = Key.Escape;

        private bool _isMenuOpen;

        private void Awake()
        {
            if (cameraTransform == null)
            {
                Camera mainCamera = Camera.main;
                if (mainCamera != null)
                    cameraTransform = mainCamera.transform;
                else
                    Debug.LogError("No camera found! Please assign cameraTransform manually.");
            }

            if (menuPanel != null)
                menuPanel.SetActive(false);

            _isMenuOpen = false;
        }

        private void OnEnable()
        {
            if (menuAction.action != null)
            {
                menuAction.action.Enable();
                menuAction.action.performed += OnMenuActionPerformed;
            }

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            if (menuAction.action != null)
            {
                menuAction.action.performed -= OnMenuActionPerformed;
                menuAction.action.Disable();
            }

            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void Update()
        {
            if (enableKeyboardInput && Keyboard.current != null)
            {
                if (Keyboard.current[keyboardToggleKey].wasPressedThisFrame)
                {
                    ToggleMenu();
                }
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name != "GameScene")
            {
                gameObject.SetActive(false);
            }
        }

        private void OnMenuActionPerformed(InputAction.CallbackContext context)
        {
            ToggleMenu();
        }

        public void ToggleMenu()
        {
            if (confirmationDialog != null && confirmationDialog.IsVisible)
            {
                confirmationDialog.Hide();
                return;
            }

            _isMenuOpen = !_isMenuOpen;

            if (menuPanel != null)
            {
                menuPanel.SetActive(_isMenuOpen);

                if (_isMenuOpen)
                {
                    PositionMenuInFrontOfCamera();
                    SetPauseState(true);
                }
                else
                {
                    SetPauseState(false);
                }
            }
        }

        public void CloseMenu()
        {
            _isMenuOpen = false;
            if (menuPanel != null)
                menuPanel.SetActive(false);

            SetPauseState(false);
        }

        public void OpenMenu()
        {
            _isMenuOpen = true;
            if (menuPanel != null)
            {
                menuPanel.SetActive(true);
                PositionMenuInFrontOfCamera();
            }

            SetPauseState(true);
        }

        private void SetPauseState(bool isPaused)
        {
            if (pauseGameWhenOpen)
            {
                Time.timeScale = isPaused ? 0f : 1f;
            }
        }

        private void PositionMenuInFrontOfCamera()
        {
            if (cameraTransform == null || hudCanvas == null)
                return;

            Vector3 targetPosition = cameraTransform.position + cameraTransform.forward * distanceFromCamera + offset;
            hudCanvas.transform.position = targetPosition;
            hudCanvas.transform.rotation = Quaternion.LookRotation(hudCanvas.transform.position - cameraTransform.position);
        }
    }
}

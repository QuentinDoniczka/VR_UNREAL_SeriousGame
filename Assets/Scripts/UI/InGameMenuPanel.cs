using UnityEngine;
using UnityEngine.UI;
using Core.Managers;

namespace UI
{
    public class InGameMenuPanel : MonoBehaviour
    {
        [Header("Button References")]
        [SerializeField] private Button menuButton;
        [SerializeField] private Button backButton;

        [Header("Dependencies")]
        [SerializeField] private ConfirmationDialog confirmationDialog;

        private void Start()
        {
            SetupButtons();
        }

        private void SetupButtons()
        {
            if (menuButton != null)
                menuButton.onClick.AddListener(OnMenuButtonClicked);

            if (backButton != null)
                backButton.onClick.AddListener(OnBackButtonClicked);
        }

        private void OnMenuButtonClicked()
        {
            if (confirmationDialog != null)
            {
                confirmationDialog.Show(
                    onConfirm: ReturnToMainMenu,
                    onCancel: null
                );
            }
            else
            {
                Debug.LogWarning("ConfirmationDialog reference missing. Returning to menu without confirmation.");
                ReturnToMainMenu();
            }
        }

        private void ReturnToMainMenu()
        {
            if (SceneManager.Instance != null)
            {
                SceneManager.Instance.LoadMainMenuScene();
            }
            else
            {
                Debug.LogError("SceneManager instance not found!");
            }
        }

        private void OnBackButtonClicked()
        {
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            if (menuButton != null)
                menuButton.onClick.RemoveListener(OnMenuButtonClicked);

            if (backButton != null)
                backButton.onClick.RemoveListener(OnBackButtonClicked);
        }
    }
}

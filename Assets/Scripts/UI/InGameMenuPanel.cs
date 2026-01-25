using UnityEngine;
using UnityEngine.UI;
using Core.Managers;

namespace UI
{
    public class InGameMenuPanel : UIPanel
    {
        [Header("Button References")]
        [SerializeField] private Button menuButton;
        [SerializeField] private Button backButton;

        [Header("Dependencies")]
        [SerializeField] private ConfirmationDialog confirmationDialog;
        [SerializeField] private InGameMenuHUD menuHUD;

        protected override void RegisterButtons()
        {
            RegisterButton(menuButton, OnMenuButtonClicked);
            RegisterButton(backButton, OnBackButtonClicked);
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
            Time.timeScale = 1f;

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
            if (menuHUD != null)
            {
                menuHUD.CloseMenu();
            }
            else
            {
                Debug.LogWarning("InGameMenuPanel: menuHUD reference is missing. Assign it in the Inspector.");
                Time.timeScale = 1f;
                gameObject.SetActive(false);
            }
        }
    }
}

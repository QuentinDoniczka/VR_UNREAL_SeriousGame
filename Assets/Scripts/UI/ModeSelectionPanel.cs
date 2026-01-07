using UnityEngine;
using UnityEngine.UI;
using Core.Managers;

namespace UI
{
    public class ModeSelectionPanel : MonoBehaviour
    {
        [Header("Button References")]
        [SerializeField] private Button extincteurButton;
        [SerializeField] private Button backButton;

        [Header("Dependencies")]
        [SerializeField] private MenuPanelManager panelManager;

        private void Start()
        {
            SetupButtons();
        }

        private void SetupButtons()
        {
            if (extincteurButton != null)
                extincteurButton.onClick.AddListener(OnExtincteurClicked);

            if (backButton != null)
                backButton.onClick.AddListener(OnBackButtonClicked);
        }

        private void OnExtincteurClicked()
        {
            if (SceneManager.Instance != null)
            {
                SceneManager.Instance.LoadGameScene();
            }
            else
            {
                Debug.LogError("SceneManager instance not found!");
            }
        }

        private void OnBackButtonClicked()
        {
            if (panelManager != null)
            {
                panelManager.ShowMainMenu();
            }
            else
            {
                Debug.LogError("MenuPanelManager reference is missing!");
            }
        }

        private void OnDestroy()
        {
            if (extincteurButton != null)
                extincteurButton.onClick.RemoveListener(OnExtincteurClicked);

            if (backButton != null)
                backButton.onClick.RemoveListener(OnBackButtonClicked);
        }
    }
}

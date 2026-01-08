using UnityEngine;
using UnityEngine.UI;
using Core.Managers;

namespace UI
{
    public class ModeSelectionPanel : UIPanel
    {
        [Header("Button References")]
        [SerializeField] private Button extincteurButton;
        [SerializeField] private Button backButton;

        [Header("Dependencies")]
        [SerializeField] private MenuPanelManager panelManager;

        protected override void RegisterButtons()
        {
            RegisterButton(extincteurButton, OnExtincteurClicked);
            RegisterButton(backButton, OnBackButtonClicked);
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
    }
}

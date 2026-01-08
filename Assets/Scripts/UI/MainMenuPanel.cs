using UnityEngine;
using UnityEngine.UI;
using Core.Managers;

namespace UI
{
    public class MainMenuPanel : UIPanel
    {
        [Header("Button References")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button optionsButton;
        [SerializeField] private Button quitButton;

        [Header("Dependencies")]
        [SerializeField] private MenuPanelManager panelManager;

        protected override void RegisterButtons()
        {
            RegisterButton(playButton, OnPlayButtonClicked);
            RegisterButton(optionsButton, OnOptionsButtonClicked);
            RegisterButton(quitButton, OnQuitButtonClicked);
        }

        private void OnPlayButtonClicked()
        {
            if (panelManager != null)
            {
                panelManager.ShowModeSelection();
            }
            else
            {
                Debug.LogError("MenuPanelManager reference is missing!");
            }
        }

        private void OnOptionsButtonClicked()
        {
            Debug.Log("Options - Not implemented yet");
        }

        private void OnQuitButtonClicked()
        {
            if (SceneManager.Instance != null)
            {
                SceneManager.Instance.QuitGame();
            }
            else
            {
                Debug.LogError("SceneManager instance not found!");
            }
        }
    }
}

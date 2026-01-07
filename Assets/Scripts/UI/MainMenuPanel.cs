using UnityEngine;
using UnityEngine.UI;
using Core.Managers;

namespace UI
{
    public class MainMenuPanel : MonoBehaviour
    {
        [Header("Button References")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button optionsButton;
        [SerializeField] private Button quitButton;

        [Header("Dependencies")]
        [SerializeField] private MenuPanelManager panelManager;

        private void Start()
        {
            SetupButtons();
        }

        private void SetupButtons()
        {
            if (playButton != null)
                playButton.onClick.AddListener(OnPlayButtonClicked);

            if (optionsButton != null)
                optionsButton.onClick.AddListener(OnOptionsButtonClicked);

            if (quitButton != null)
                quitButton.onClick.AddListener(OnQuitButtonClicked);
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

        private void OnDestroy()
        {
            if (playButton != null)
                playButton.onClick.RemoveListener(OnPlayButtonClicked);

            if (optionsButton != null)
                optionsButton.onClick.RemoveListener(OnOptionsButtonClicked);

            if (quitButton != null)
                quitButton.onClick.RemoveListener(OnQuitButtonClicked);
        }
    }
}

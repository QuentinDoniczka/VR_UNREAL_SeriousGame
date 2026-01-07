using UnityEngine;
using UnityEngine.UI;
using Core.Managers;

namespace UI
{
    public class MenuUI : MonoBehaviour
    {
        private const string GAME_SCENE_NAME = "GameMenuScene";

        [Header("Button References")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button optionsButton;
        [SerializeField] private Button quitButton;

        [Header("Settings")]
        [SerializeField] private bool enableButtonSounds = false;

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

        public void OnPlayButtonClicked()
        {
            if (SceneManager.Instance != null)
            {
                SceneManager.Instance.LoadScene(GAME_SCENE_NAME);
            }
            else
            {
                Debug.LogError("SceneManager instance not found!");
            }
        }

        public void OnOptionsButtonClicked()
        {
            Debug.Log("Options - Not implemented yet");
        }

        public void OnQuitButtonClicked()
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

using UnityEngine;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

namespace Core.Managers
{
    public class SceneManager : MonoBehaviour
    {
        private const string GAME_SCENE_NAME = "GameScene";
        private const string MAIN_MENU_SCENE_NAME = "MainMenuScene";

        private static SceneManager _instance;
        public static SceneManager Instance => _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this);
                return;
            }

            _instance = this;
        }

        public void LoadGameScene()
        {
            LoadScene(GAME_SCENE_NAME);
        }

        public void LoadMainMenuScene()
        {
            LoadScene(MAIN_MENU_SCENE_NAME);
        }

        public void LoadScene(string sceneName)
        {
            UnitySceneManager.LoadScene(sceneName);
        }

        public void ReloadCurrentScene()
        {
            UnitySceneManager.LoadScene(UnitySceneManager.GetActiveScene().name);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}

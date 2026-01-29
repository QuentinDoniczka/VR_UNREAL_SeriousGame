using UnityEngine;
using TMPro;

namespace UI
{
    public class ScoreHUD : MonoBehaviour
    {
        private static ScoreHUD _instance;
        public static ScoreHUD Instance => _instance;

        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI scoreText;

        [Header("Display Settings")]
        [SerializeField] private string scoreFormat = "Feux éteints: {0}";

        private int _score;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;

            UpdateDisplay();
        }

        public void AddScore(int points = 1)
        {
            _score += points;
            UpdateDisplay();
        }

        public void ResetScore()
        {
            _score = 0;
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            if (scoreText != null)
                scoreText.text = string.Format(scoreFormat, _score);
        }

        public int Score => _score;
    }
}

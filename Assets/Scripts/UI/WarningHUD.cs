using UnityEngine;
using TMPro;

namespace UI
{
    public class WarningHUD : MonoBehaviour
    {
        private static WarningHUD _instance;
        public static WarningHUD Instance => _instance;

        [Header("UI References")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TextMeshProUGUI warningText;

        [Header("Display Settings")]
        [SerializeField] private float displayDuration = 5f;
        [SerializeField] private float fadeSpeed = 3f;

        private float _hideTimer;
        private bool _isVisible;
        private string _lastMessage;
        private float _lastMessageTime;
        private const float MessageCooldown = 1f;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;

            if (canvasGroup != null)
                canvasGroup.alpha = 0f;
        }

        private void Update()
        {
            UpdateVisibility();
        }

        private void UpdateVisibility()
        {
            if (canvasGroup == null) return;

            if (_isVisible)
            {
                _hideTimer -= Time.deltaTime;
                if (_hideTimer <= 0f)
                    _isVisible = false;

                canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, 1f, fadeSpeed * Time.deltaTime);
            }
            else
            {
                canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, 0f, fadeSpeed * Time.deltaTime);
            }
        }

        public void ShowWarning(string message)
        {
            if (string.IsNullOrEmpty(message)) return;

            if (message == _lastMessage && Time.time - _lastMessageTime < MessageCooldown)
                return;

            _lastMessage = message;
            _lastMessageTime = Time.time;

            if (warningText != null)
                warningText.text = message;

            _hideTimer = displayDuration;
            _isVisible = true;
        }
    }
}

using UnityEngine;
using TMPro;

namespace Interaction.Inspection
{
    public class InspectionHUD : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI detailsText;

        [Header("Fade Settings")]
        [SerializeField] private float fadeSpeed = 5f;
        [SerializeField] private float targetAlpha = 0.85f;

        private bool _isVisible;
        private float _currentAlpha;

        private void Awake()
        {
            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>();

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                _currentAlpha = 0f;
            }
        }

        private void Update()
        {
            UpdateFade();
        }

        private void UpdateFade()
        {
            if (canvasGroup == null) return;

            float target = _isVisible ? targetAlpha : 0f;

            if (Mathf.Approximately(_currentAlpha, target)) return;

            _currentAlpha = Mathf.MoveTowards(_currentAlpha, target, fadeSpeed * Time.deltaTime);
            canvasGroup.alpha = _currentAlpha;
        }

        public void Show(string objectName, string details)
        {
            if (nameText != null)
                nameText.text = objectName;

            if (detailsText != null)
                detailsText.text = details;

            _isVisible = true;
        }

        public void Hide()
        {
            _isVisible = false;
        }
    }
}

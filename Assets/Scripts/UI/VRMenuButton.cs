using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UI
{
    [RequireComponent(typeof(Button))]
    public class VRMenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Visual Feedback")]
        [SerializeField] private float hoverScale = 1.1f;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color hoverColor = new Color(0.8f, 0.9f, 1f);
        [SerializeField] private float transitionSpeed = 10f;

        private Button _button;
        private Image _buttonImage;
        private Vector3 _originalScale;
        private Vector3 _targetScale;
        private Color _targetColor;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _buttonImage = GetComponent<Image>();
            _originalScale = transform.localScale;
            _targetScale = _originalScale;
            _targetColor = normalColor;

            if (_buttonImage != null)
            {
                _buttonImage.color = normalColor;
            }
        }

        private void Update()
        {
            transform.localScale = Vector3.Lerp(transform.localScale, _targetScale, Time.deltaTime * transitionSpeed);

            if (_buttonImage != null)
            {
                _buttonImage.color = Color.Lerp(_buttonImage.color, _targetColor, Time.deltaTime * transitionSpeed);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_button != null && _button.interactable)
            {
                _targetScale = _originalScale * hoverScale;
                _targetColor = hoverColor;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _targetScale = _originalScale;
            _targetColor = normalColor;
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using System;

namespace UI
{
    public class ConfirmationDialog : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject dialogPanel;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;

        private Action _onConfirm;
        private Action _onCancel;

        private void Start()
        {
            SetupButtons();
            Hide();
        }

        private void SetupButtons()
        {
            if (confirmButton != null)
                confirmButton.onClick.AddListener(OnConfirm);

            if (cancelButton != null)
                cancelButton.onClick.AddListener(OnCancel);
        }

        public void Show(Action onConfirm, Action onCancel = null)
        {
            _onConfirm = onConfirm;
            _onCancel = onCancel;

            if (dialogPanel != null)
                dialogPanel.SetActive(true);
        }

        public void Hide()
        {
            if (dialogPanel != null)
                dialogPanel.SetActive(false);

            _onConfirm = null;
            _onCancel = null;
        }

        private void OnConfirm()
        {
            _onConfirm?.Invoke();
            Hide();
        }

        private void OnCancel()
        {
            _onCancel?.Invoke();
            Hide();
        }

        private void OnDestroy()
        {
            if (confirmButton != null)
                confirmButton.onClick.RemoveListener(OnConfirm);

            if (cancelButton != null)
                cancelButton.onClick.RemoveListener(OnCancel);
        }
    }
}

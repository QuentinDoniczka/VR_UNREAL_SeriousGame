using UnityEngine;
using UnityEngine.UI;
using System;

namespace UI
{
    public class ConfirmationDialog : UIPanel
    {
        [Header("UI References")]
        [SerializeField] private GameObject dialogPanel;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;

        private Action _onConfirm;
        private Action _onCancel;

        public bool IsVisible => dialogPanel != null && dialogPanel.activeSelf;

        protected override void RegisterButtons()
        {
            RegisterButton(confirmButton, OnConfirm);
            RegisterButton(cancelButton, OnCancel);
        }

        protected override void OnPanelStart()
        {
            Hide();
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
    }
}

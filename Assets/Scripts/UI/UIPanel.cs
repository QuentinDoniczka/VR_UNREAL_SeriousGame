using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;

namespace UI
{
    /// <summary>
    /// Base class for UI panels that handles automatic button listener setup and cleanup.
    /// Eliminates boilerplate code for adding/removing button listeners.
    /// </summary>
    public abstract class UIPanel : MonoBehaviour
    {
        private readonly List<ButtonBinding> _buttonBindings = new List<ButtonBinding>();

        private void Start()
        {
            RegisterButtons();
            SetupButtonListeners();
            OnPanelStart();
        }

        private void OnDestroy()
        {
            CleanupButtonListeners();
            OnPanelDestroy();
        }

        /// <summary>
        /// Override this to register buttons using RegisterButton().
        /// Called before button listeners are set up.
        /// </summary>
        protected abstract void RegisterButtons();

        /// <summary>
        /// Override this to add custom initialization logic.
        /// Called after button listeners are set up.
        /// </summary>
        protected virtual void OnPanelStart() { }

        /// <summary>
        /// Override this to add custom cleanup logic.
        /// Called after button listeners are removed.
        /// </summary>
        protected virtual void OnPanelDestroy() { }

        /// <summary>
        /// Register a button with its callback.
        /// The listener will be automatically added in Start and removed in OnDestroy.
        /// </summary>
        protected void RegisterButton(Button button, UnityAction callback)
        {
            if (button != null && callback != null)
            {
                _buttonBindings.Add(new ButtonBinding(button, callback));
            }
        }

        private void SetupButtonListeners()
        {
            foreach (var binding in _buttonBindings)
            {
                binding.Button.onClick.AddListener(binding.Callback);
            }
        }

        private void CleanupButtonListeners()
        {
            foreach (var binding in _buttonBindings)
            {
                if (binding.Button != null)
                {
                    binding.Button.onClick.RemoveListener(binding.Callback);
                }
            }
        }

        private readonly struct ButtonBinding
        {
            public readonly Button Button;
            public readonly UnityAction Callback;

            public ButtonBinding(Button button, UnityAction callback)
            {
                Button = button;
                Callback = callback;
            }
        }
    }
}

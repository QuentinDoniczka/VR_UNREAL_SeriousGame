using UnityEngine;

namespace UI
{
    public class MenuPanelManager : MonoBehaviour
    {
        [Header("Panel References")]
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject modeSelectionPanel;

        private void Start()
        {
            ShowMainMenu();
        }

        public void ShowMainMenu()
        {
            SetActivePanel(mainMenuPanel);
        }

        public void ShowModeSelection()
        {
            SetActivePanel(modeSelectionPanel);
        }

        private void SetActivePanel(GameObject panelToShow)
        {
            if (mainMenuPanel != null)
                mainMenuPanel.SetActive(mainMenuPanel == panelToShow);

            if (modeSelectionPanel != null)
                modeSelectionPanel.SetActive(modeSelectionPanel == panelToShow);
        }
    }
}

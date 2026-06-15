using UnityEngine;
using UnityEngine.UI;

public class ControllerMenu : MonoBehaviour
{
    [Header("Controls subpanels")]
    [SerializeField] private GameObject keyboardPanel;
    [SerializeField] private GameObject gamepadPanel;


    private void OnEnable()
    {
        ShowKeyboardLayout();
    }

    public void ShowKeyboardLayout()
    {
        if (keyboardPanel != null) keyboardPanel.SetActive(true);
        if (gamepadPanel != null) gamepadPanel.SetActive(false);
    }

    public void ShowGamepadLayout()
    {
        if (keyboardPanel != null) keyboardPanel.SetActive(false);
        if (gamepadPanel != null) gamepadPanel.SetActive(true);
    }
}

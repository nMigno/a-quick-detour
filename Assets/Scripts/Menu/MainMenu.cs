using UnityEngine;
using UnityEngine.InputSystem;

public enum SubMenu
{
    CONTROLLER,
    SOUND,
    GRAPHICS
}

public class MainMenu : MonoBehaviour
{
    [Header("Main Configuration")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private AudioClip menuMusic;

    [Header("Options Menu")]
    [SerializeField] private GameObject mainOptionsMenu;
    [SerializeField] private GameObject controllerMenu;
    [SerializeField] private GameObject soundMenu;
    [SerializeField] private GameObject graphicsMenu;

    [Header("First Selected Buttons")]
    [SerializeField] private GameObject mainFirstButton;
    [SerializeField] private GameObject creditsFirstButton;
    [SerializeField] private GameObject optionsFirstButton;

    void Start()
    {
        if (menuMusic != null && GameManager.Instance != null)
        {
            GameManager.Instance.TogglePauseMusic(false); 
            GameManager.Instance.PlayMusic(menuMusic);
        }

        OpenMainMenuPanel();
        OpenSubMenu(null);
    }

    public void OpenMainMenuPanel()
    {
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
        creditsPanel.SetActive(false);

        if (mainFirstButton != null)
        {
            GameManager.Instance.SetFirstSelectedElement(mainFirstButton);
        }
    }

    void OpenSubMenu(SubMenu? subMenu)
    {
        controllerMenu.SetActive(SubMenu.CONTROLLER == subMenu);
        soundMenu.SetActive(SubMenu.SOUND == subMenu);
        //graphicsMenu.SetActive(SubMenu.GRAPHICS == subMenu);
    }


    public void OpenOptionsPanel()
    {
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);

        if (mainOptionsMenu != null) mainOptionsMenu.SetActive(true);

        OpenSubMenu(SubMenu.CONTROLLER);

        if (optionsFirstButton != null)
        {
            GameManager.Instance.SetFirstSelectedElement(optionsFirstButton);
        }
    }

    public void CloseOptionsPanel()
    {
        optionsMenu.SetActive(false);
        mainMenu.SetActive(true);

        if (mainFirstButton != null)
        {
            GameManager.Instance.SetFirstSelectedElement(mainFirstButton);
        }

    }

    
    public void OpenControllerMenu()
    {
        OpenSubMenu(SubMenu.CONTROLLER);
    }
    public void OpenSoundMenu()
    {
        OpenSubMenu(SubMenu.SOUND);
    }
    public void OpenGraphicsMenu()
    {
        OpenSubMenu(SubMenu.GRAPHICS);
    }
    
    public void OpenCreditsPanel()
    {
        mainMenu.SetActive(false);
        creditsPanel.SetActive(true);

        if (creditsFirstButton != null)
        {
            GameManager.Instance.SetFirstSelectedElement(creditsFirstButton);
        }
    }

    public void CloseCreditsPanel()
    {
        creditsPanel.SetActive(false); 
        mainMenu.SetActive(true); 

        if (mainFirstButton != null)
        {
            GameManager.Instance.SetFirstSelectedElement(mainFirstButton);
        }
    }

    public void QuitGame()
    {
        GameManager.Instance.QuitGame();
    }

    public void PlayGame()
    {
        GameManager.Instance.StartGame();
    }
}

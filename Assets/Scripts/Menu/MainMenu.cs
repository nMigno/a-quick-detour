using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.UI;

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

    [Header("Start Game Transition")]
    [Tooltip("Black Image that covers screen")]
    [SerializeField] private Image fadeOverlay;
    [Tooltip("Fade duration towards black (seconds)")]
    [SerializeField] private float startFadeDuration = 1.5f;
    [Tooltip("Exclusive SFX for 'Play' button")]
    [SerializeField] private AudioClip playButtonSFX;
    [Tooltip("Local AudioSource that reproduces 'Play' SFX")]
    [SerializeField] private AudioSource menuAudioSource;

    private bool isStartingGame = false;

    void Start()
    {
        if (fadeOverlay != null) fadeOverlay.gameObject.SetActive(false);
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
        if (isStartingGame) return;

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
        // Avoids button spamming
        if (isStartingGame) return;
        isStartingGame = true;

        // Playing unique SFX
        menuAudioSource.PlayOneShot(playButtonSFX);

        // Transition
        if (fadeOverlay != null)
        {
            StartCoroutine(FadeAndStart());
        }
        else
        {
            GameManager.Instance.StartGame();
        }
    }

    private IEnumerator FadeAndStart()
    {
        UnityEngine.EventSystems.EventSystem.current.enabled = false;

        PlayerInput playerInput = FindAnyObjectByType<PlayerInput>();
        if (playerInput != null)
        {
            playerInput.DeactivateInput();
        }

        fadeOverlay.gameObject.SetActive(true);
        Color fadeColor = fadeOverlay.color;
        fadeColor.a = 0f;
        fadeOverlay.color = fadeColor;
        
        GameManager.Instance.FadeOutMusic(startFadeDuration);

        float elapsed = 0f;
        while (elapsed < startFadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            fadeColor.a = Mathf.Clamp01(elapsed / startFadeDuration);
            fadeOverlay.color = fadeColor;
            yield return null;
        }

        fadeColor.a = 1f;
        fadeOverlay.color = fadeColor;

        yield return new WaitForSecondsRealtime(0.5f);

        GameManager.Instance.StartGame();
    }
}

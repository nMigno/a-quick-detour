using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections;

public class EmergencyUI : MonoBehaviour
{
    [Header("Pause Menu")]
    public GameObject pausePanel;
    public GameObject pauseFirstButton;
    private bool isPaused = false;

    [Header("Level End")]
    public GameObject toBeContinuedPanel;
    public GameObject tbcFirstButton;

    [Header("Input")]
    public InputActionReference pauseAction;

    private void Awake()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (toBeContinuedPanel != null) toBeContinuedPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    private void OnEnable()
    {
        if (pauseAction != null)
        {
            pauseAction.action.Enable();
            pauseAction.action.performed += OnPauseInput;
        }
    }

    private void OnDisable()
    {
        if (pauseAction != null)
        {
            pauseAction.action.performed -= OnPauseInput;
            pauseAction.action.Disable();
        }
    }

    private void OnPauseInput(InputAction.CallbackContext context)
    {
        if (toBeContinuedPanel != null && toBeContinuedPanel.activeSelf) return;
        TogglePause();
    }
    public void TogglePause()
    {
        isPaused = !isPaused;
        pausePanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.TogglePauseMusic(isPaused);
        }

        if (isPaused)
        {
            if (pauseFirstButton != null)
            {
                GameManager.Instance.SetFirstSelectedElement(pauseFirstButton);
            }
            GameManager.Instance.ActivateActionMap(GameInputMap.UI);
        }
        else
        {
            GameManager.Instance.ActivateActionMap(GameInputMap.Gameplay);
        }
    }
    public void ResumeGame()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1f;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.TogglePauseMusic(false); 
        }
        
        GameManager.Instance.ActivateActionMap(GameInputMap.Gameplay);
    }
    public void BackToMainMenu()
    {
        Time.timeScale = 1f;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetGameState();
        }

        SceneManager.LoadScene("MainMenu");
    }
    public void TriggerLevelEnd()
    {
        StartCoroutine(EndLevelRoutine());
    }

    private IEnumerator EndLevelRoutine()
    {
        Time.timeScale = 0f;
        toBeContinuedPanel.SetActive(true);
        GameManager.Instance.SetFirstSelectedElement(tbcFirstButton);
        GameManager.Instance.ActivateActionMap(GameInputMap.UI);

        yield return new WaitForSecondsRealtime(5f);

        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}

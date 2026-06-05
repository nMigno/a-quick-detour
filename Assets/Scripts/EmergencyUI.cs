using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections;

public class EmergencyUI : MonoBehaviour
{
    [Header("Pause Menu")]
    public GameObject pausePanel;
    private bool isPaused = false;

    [Header("Level End")]
    public GameObject toBeContinuedPanel;

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
    }
    public void ResumeGame()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }
    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
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

        yield return new WaitForSecondsRealtime(5f);

        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}

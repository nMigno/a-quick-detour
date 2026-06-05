using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum GameInputMap { UI, Gameplay }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Player Stats")]

    [field: SerializeField] public int maxHP { get; private set; } = 2;
    [field: SerializeField] public int currentHP { get; private set; } = 2;

    [Header("Power-ups & Skills")]
    public bool hasFlashlightUpgrade = false;
    public bool hasDoubleJump = false;
    public bool hasBlanketGlider = false;

    [Header("Checkpoint System")]
    public string lastCheckpointScene;
    public Vector2 lastCheckpointPosition;

    [Header("Door Transitions")]
    public string doorID = "";

    [Header("World Persistence")]
    public List<string> revealedIllusionWalls = new List<string>();
    public List<string> deadEnemies = new List<string>();
    public List<string> collectedBatteries = new List<string>();

    [Header("Flashlight Battery Manager")]
    public int currentFlashlightUses = 2;
    public int checkpointFlashlightUses = 2;

    [Header("Input Action Maps")]
    [SerializeField] private string uiActionMap = "UI";
    [SerializeField] private string gameplayActionMap = "Gameplay";
    [SerializeField] private InputActionReference navigateAction;

    [Header("Audio")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("SoundSettings")]
    public float currentMusicVolume = 10f;
    public float currentSFXVolume = 10f;

    [Header("UI Sound Effects")]
    [SerializeField] private AudioClip uiConfirmClip;
    [SerializeField] private AudioClip uiCancelClip;
    [SerializeField] private AudioSource uiAudioSource;

    // We'll use this later
    //[Header("Sound Variation")]
    //[SerializeField, Range(0.1f, 3f)] private float minPitch = 0.95f;
    //[SerializeField, Range(0.1f, 3f)] private float maxPitch = 1.05f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Debug.Log("Loading Main Menu...");
        SceneManager.LoadScene("MainMenu");
    }

    public void RegisterRevealedWall(string wallID)
    {
        if (!revealedIllusionWalls.Contains(wallID))
        {
            revealedIllusionWalls.Add(wallID);
            Debug.Log($"GameManager: Muro {wallID} registrado como destruido.");
        }
    }
    public void ActivateActionMap(GameInputMap actionMapName)
    {
        switch (actionMapName)
        {
            case GameInputMap.UI:
                InputSystem.actions.FindActionMap(gameplayActionMap).Disable();
                InputSystem.actions.FindActionMap(uiActionMap).Enable();
                break;
            case GameInputMap.Gameplay:
                InputSystem.actions.FindActionMap(uiActionMap).Disable();
                InputSystem.actions.FindActionMap(gameplayActionMap).Enable();
                break;
        }
    }
    public void RegisterDeadEnemy(string EnemyID)
    {
        if (!deadEnemies.Contains(EnemyID))
        {
            deadEnemies.Add(EnemyID);
            Debug.Log($"GameManager: Enemy {EnemyID} registered as dead.");
        }
    }

    public void UseDoorTransition(string sceneToLoad, string destinationDoorID)
    {
        doorID = destinationDoorID;
        SceneManager.LoadScene(sceneToLoad);
    }

    public void UnlockFlashlightUpgrade()
    {
        hasFlashlightUpgrade = true;
        Debug.Log("Flashlight upgrade unlocked.");
    }
    public void UnlockDoubleJump()
    {
        hasDoubleJump = true;
        Debug.Log("Double Jump unlocked!");
    }

    public void UnlockBlanketGlider()
    {
        hasBlanketGlider = true;
        Debug.Log("Blanket glider unlocked.");
    }

    public void UpdateHP(int newHP)
    {
        currentHP = Mathf.Clamp(newHP, 0, maxHP);
    }

    public void SetCheckpoint(Vector2 position, string sceneName)
    {
        lastCheckpointPosition = position;
        lastCheckpointScene = sceneName;
        checkpointFlashlightUses = currentFlashlightUses;

        Debug.Log($"GameManager: Checkpoint set at {position} in scene {sceneName}");
    }

    public void RespawnPlayer()
    {
        Debug.Log("GameManager: Respawning on last checkpoint.");

        UpdateHP(maxHP);

        currentFlashlightUses = checkpointFlashlightUses;

        SceneManager.LoadScene(lastCheckpointScene);
    }

    public void RegisterCollectedBattery(string batteryID)
    {
        if (!collectedBatteries.Contains(batteryID))
        {
            collectedBatteries.Add(batteryID);
            Debug.Log($"GameManager: Battery {batteryID} collected!");
        }
    }

    public void PlayUIConfirmSound()
    {
        Debug.Log("Confirm UI Sound");
    }

    public void PlayUICancelSound()
    {
        Debug.Log("Cancel UI Sound");
    }

    public void SetFirstSelectedElement(GameObject firstElement)
    {
        if (UnityEngine.EventSystems.EventSystem.current != null)
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(firstElement);
        }
    }

    public void StartGame()
    {
        GameManager.Instance.PlayUIConfirmSound();

        string firstLevel = "Level-1";
        lastCheckpointScene = firstLevel;

        currentHP = maxHP;
        currentFlashlightUses = 2;

        UnityEngine.SceneManagement.SceneManager.LoadScene(firstLevel);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }

    public void LoadCredits()
    {
        Debug.Log("Loading credits...");
    }
}

using System.Collections.Generic;
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
    public bool hasBlanketBase = false;
    public bool hasFlashlight = true;

    [Header("Checkpoint System")]
    public string lastCheckpointScene;
    public Vector2 lastCheckpointPosition;

    [Header("Door Transitions")]
    public string doorID = "";

    [Header("World Persistence")]
    public List<string> revealedIllusionWalls = new List<string>();
    public List<string> savedRevealedIllusionWalls = new List<string>();
    public List<DeadEnemyData> deadEnemies = new List<DeadEnemyData>();
    public List<DeadEnemyData> savedDeadEnemies = new List<DeadEnemyData>();
    public List<string> collectedBatteries = new List<string>();

    [Header("Flashlight Battery Manager")]
    public int currentFlashlightUses = 2;
    public int checkpointFlashlightUses = 2;

    [Header("Input Action Maps")]
    [SerializeField] private string uiActionMap = "UI";
    [SerializeField] private string gameplayActionMap = "Player";
    [SerializeField] private InputActionReference navigateAction;

    [Header("Audio")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Sound Settings")]
    [Range (0.0001f, 1f)] public float currentMusicVolume = 1f;
    [Range (0.0001f, 1f)] public float currentSFXVolume = 1f;

    [Header("Music Settings")]
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private float pauseVolumeReduction = -12f;

    [Header("UI Sound Effects")]
    [SerializeField] private AudioClip uiConfirmClip;
    [SerializeField] private AudioClip uiCancelClip;
    [SerializeField] private AudioClip uiHoverClip;
    [SerializeField] private AudioSource uiAudioSource;
    private bool isAutoSelecting = false;

    [Header("Sound Variation")]
    [SerializeField, Range(0.1f, 3f)] private float minPitch = 0.9f;
    [SerializeField, Range(0.1f, 3f)] private float maxPitch = 1.1f;

    [Header("Scene Debug Settings")]
    [Tooltip("Exact scene name")]
    [SerializeField] private string startingScene = "Level-1";

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
        if (audioMixer != null)
        {
            UpdateMusicVolume(currentMusicVolume);
            UpdateSFXVolume(currentSFXVolume);
        }

        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "Boot")
        {
            Debug.Log("Loading Main Menu...");
            SceneManager.LoadScene("MainMenu");
        }
        else if (currentScene != "MainMenu")
        {
            if (string.IsNullOrEmpty(lastCheckpointScene))
            {
                Debug.Log("Debug Scene Start");
                ResetGameState();
                lastCheckpointScene = currentScene;
            }
        }
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
        PlayerInput playerInput = FindAnyObjectByType<PlayerInput>();

        if (playerInput != null)
        {
            switch (actionMapName)
            {
                case GameInputMap.UI:
                    playerInput.SwitchCurrentActionMap(uiActionMap);
                    break;
                case GameInputMap.Gameplay:
                    playerInput.SwitchCurrentActionMap(gameplayActionMap);
                    break;
            }
        }

    }
    public void RegisterDeadEnemy(string EnemyID, Vector3 deathPosition)
    {
        if (!deadEnemies.Exists(e => e.enemyID == EnemyID))
        {
            deadEnemies.Add(new DeadEnemyData(EnemyID, deathPosition));
            Debug.Log($"GameManager: Enemy {EnemyID} registered as dead at {deathPosition}.");
        }
    }

    public bool IsEnemyDead(string enemyID, out Vector3 savedPosition)
    {
        DeadEnemyData data = deadEnemies.Find(e => e.enemyID == enemyID);
        if (data != null)
        {
            savedPosition = data.position;
            return true;
        }
        
        savedPosition = Vector3.zero;
        return false;
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
        
        savedDeadEnemies.Clear();
        foreach(var enemy in deadEnemies) 
        {
            savedDeadEnemies.Add(new DeadEnemyData(enemy.enemyID, enemy.position));
        }

        savedRevealedIllusionWalls.Clear();
        savedRevealedIllusionWalls.AddRange(revealedIllusionWalls);

        Debug.Log($"GameManager: Checkpoint set at {position} in scene {sceneName}");
    }

    public void RespawnPlayer()
    {
        Debug.Log("GameManager: Respawning on last checkpoint.");

        deadEnemies.Clear();
        foreach(var enemy in savedDeadEnemies) 
        {
            deadEnemies.Add(new DeadEnemyData(enemy.enemyID, enemy.position));
        }

        revealedIllusionWalls.Clear();
        revealedIllusionWalls.AddRange(savedRevealedIllusionWalls);

        UpdateHP(maxHP);

        if (!string.IsNullOrEmpty(lastCheckpointScene))
        {
            SceneManager.LoadScene(lastCheckpointScene);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

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
        if (uiAudioSource != null && uiConfirmClip != null)
        {
            uiAudioSource.pitch = Random.Range(minPitch, maxPitch);
            uiAudioSource.PlayOneShot(uiConfirmClip);
        }
    }

    public void PlayUICancelSound()
    {
        if (uiAudioSource != null && uiCancelClip != null)
        {
            uiAudioSource.pitch = Random.Range(minPitch, maxPitch);
            uiAudioSource.PlayOneShot(uiCancelClip);
        }
    }

    public void PlayUIHoverSound()
    {
        if (isAutoSelecting) return;

        if (uiAudioSource != null && uiHoverClip != null)
        {
            uiAudioSource.pitch = Random.Range(minPitch, maxPitch);
            uiAudioSource.PlayOneShot(uiHoverClip);
        }
    }

    public void PlayMusic(AudioClip newClip)
    {
        if (newClip == null)
        {
            musicAudioSource.Stop();
            musicAudioSource.clip = null;
            return;
        }

        if (musicAudioSource.clip == newClip) return;

        musicAudioSource.clip = newClip;
        musicAudioSource.Play();
    }

    public void UpdateMusicVolume(float volume)
    {
        currentMusicVolume = volume;        
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(currentMusicVolume) * 20);
    }

    public void UpdateSFXVolume(float volume)
    {
        currentSFXVolume = volume;
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(currentSFXVolume) * 20);
    }

    public void TogglePauseMusic(bool isPaused)
    {
        if (isPaused)
        {
            audioMixer.SetFloat("MusicVolume", (Mathf.Log10(currentMusicVolume) * 20) + pauseVolumeReduction);
        }
        else
        {
            UpdateMusicVolume(currentMusicVolume);
        }
    }

    public void SetFirstSelectedElement(GameObject firstElement)
    {
        if (UnityEngine.EventSystems.EventSystem.current != null)
        {
            isAutoSelecting = true;

            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(firstElement);

            isAutoSelecting = false;
        }
    }

    public void StartGame()
    {
        ResetGameState();

        if (string.IsNullOrEmpty(startingScene))
        {
        Debug.LogError("Build error: No assigned scene in 'Starting Scene' on Game Manager Inspector.");
        return;
        }

        lastCheckpointScene = startingScene;
        currentHP = maxHP;
        currentFlashlightUses = 2;
        SceneManager.LoadScene(startingScene);
    }

    public void ResetGameState()
    {
        currentHP = maxHP;
        currentFlashlightUses = 2;
        checkpointFlashlightUses = 2;

        lastCheckpointScene = "";
        lastCheckpointPosition = Vector2.zero;
        doorID = "";

        revealedIllusionWalls.Clear();
        savedRevealedIllusionWalls.Clear();
        deadEnemies.Clear();
        savedDeadEnemies.Clear();
        collectedBatteries.Clear();

        hasFlashlightUpgrade = false;
        hasDoubleJump = false;
        hasBlanketGlider = false;
        hasBlanketBase = false;
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

    #if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying && audioMixer != null)
        {
            UpdateMusicVolume(currentMusicVolume);
            UpdateSFXVolume(currentSFXVolume);
        }
    }
    #endif

    public void RepairFlashlight()
    {
        hasFlashlight = true;
        Debug.Log("Flashlight repaired!");
    }

    public void UnlockBlanketBase()
    {
        hasBlanketBase = true;
    }
}

[System.Serializable]
public class DeadEnemyData
{
    public string enemyID;
    public Vector3 position;

    public DeadEnemyData(string id, Vector3 pos)
    {
        enemyID = id;
        position = pos;
    }
}
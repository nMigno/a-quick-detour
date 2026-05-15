using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Player Stats")]

    [field: SerializeField] public int maxHP { get; private set; } = 2;
    [field: SerializeField] public int currentHP { get; private set; } = 2;

    [Header("Power-ups & Skills")]
    public bool hasFlashlightUpgrade = false;
    public bool hasBlanketShield = false;
    public bool hasBlanketGlider = false;

    public bool hasKey = false;

    [Header("Checkpoint System")]
    public string lastCheckpointScene;
    public Vector2 lastCheckpointPosition;

    [Header("Door Transitions")]
    public string doorID = "";

    [Header("World Persistence")]
    public List<string> revealedIllusionWalls = new List<string>();

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
        if (string.IsNullOrEmpty(lastCheckpointScene))
        {
            lastCheckpointScene = SceneManager.GetActiveScene().name;
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

    public void UnlockKey()
    {

        hasKey = true;
        Debug.Log("Key picked up.");
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

    public void UnlockBlanketShield()
    {
        hasBlanketShield = true;   
        Debug.Log("Blanket shield unlocked.");
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
        Debug.Log($"GameManager: Checkpoint set at {position} in scene {sceneName}");
    }

    public void RespawnPlayer()
    {
        Debug.Log("GameManager: Respawning on last checkpoint.");

        UpdateHP(maxHP);
        SceneManager.LoadScene(lastCheckpointScene);
    }
}

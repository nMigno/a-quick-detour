using UnityEngine;
using Unity.Cinemachine;

public class LevelManager : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioClip levelMusic;

    [Header("References")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject flashlightUpgrade;
    [SerializeField] private GameObject doubleJumpUpgrade;

    [Header("Collectables")]
    [SerializeField] private GameObject key;

    [SerializeField] private bool disableFlashlightOnStart = false;
    [SerializeField] private bool disableBlanketOnStart = false;

    private void Awake()
    {
        if (GameManager.Instance != null)
        {
            ApplyLevelRestrictions();
        }

        PlacePlayer();
    }

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayMusic(levelMusic);
        }

        SyncSceneStates();
    }

    private void ApplyLevelRestrictions()
    {
        bool isFreshStart = !string.IsNullOrEmpty(GameManager.Instance.doorID) || GameManager.Instance.lastCheckpointPosition == Vector2.zero;
        if (!isFreshStart) return;

        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        if (currentScene == "Level-2" || disableFlashlightOnStart)
        {
            GameManager.Instance.hasFlashlight = false;
            GameManager.Instance.savedHasFlashlight = false;
            Debug.Log("LevelManager: Flashlight  disabled on Level-2.");
        }

        if (currentScene == "Level-2" || disableBlanketOnStart)
        {
            if (!GameManager.Instance.hasBlanketGlider)
            {
                GameManager.Instance.hasBlanketGlider = false;
                GameManager.Instance.hasBlanketBase = false;

                GameManager.Instance.savedHasBlanketGlider = false;
                GameManager.Instance.savedHasBlanketBase = false;
                Debug.Log("LevelManager: Blanket disabled on Level-2.");
            }
        }
    }

    private void PlacePlayer()
    {
        Vector3 startPos = player.transform.position;
        bool playerMoved = false;

        // Door transition check
        if (!string.IsNullOrEmpty(GameManager.Instance.doorID))
        {
            Door[] doorsInScene = FindObjectsByType<Door>(FindObjectsSortMode.None);

            for (int i = 0; i < doorsInScene.Length && !playerMoved; i++)
            {
                if (doorsInScene[i].myDoorID == GameManager.Instance.doorID)
                {
                    Transform doorSpawnPoint = doorsInScene[i].playerSpawn;
                    player.transform.position = doorSpawnPoint != null ? doorSpawnPoint.position : doorsInScene[i].transform.position;

                    string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
                    GameManager.Instance.SetCheckpoint(player.transform.position, currentScene);
                    Debug.Log($"LevelManager: Checkpoint generated at {GameManager.Instance.doorID} of {currentScene}");

                    GameManager.Instance.doorID = "";
                    playerMoved = true;
                }
            }

            if (!playerMoved) Debug.LogWarning("Door ID not found in scene");
        }
        // If no door, checkpoint check
        else if (GameManager.Instance.lastCheckpointPosition != Vector2.zero)
        {
            player.transform.position = GameManager.Instance.lastCheckpointPosition;
            playerMoved = true;
        }
        else
        {
            GameObject spawnPoint = GameObject.Find("SpawnPoint");
            if (spawnPoint != null)
            {
                player.transform.position = spawnPoint.transform.position;
                playerMoved = true;
            }
            else
            {
                playerMoved = true;
            }
        }

        // Force player and camera TP
        if (playerMoved)
        {
            Physics2D.SyncTransforms();

            CinemachineCamera cam = FindAnyObjectByType<CinemachineCamera>();
            if (cam != null)
            {
                // Cinemachine update
                Vector3 warpDelta = player.transform.position - startPos;
                cam.OnTargetObjectWarped(player.transform, warpDelta);
                cam.PreviousStateIsValid = false;
            }
        }
    }

    private void SyncSceneStates()
    {
        if (GameManager.Instance.hasFlashlightUpgrade && flashlightUpgrade != null)
        {
            flashlightUpgrade.SetActive(false);
        }

        if (GameManager.Instance.hasDoubleJump && doubleJumpUpgrade != null)
        {
            doubleJumpUpgrade.SetActive(false);
        }

        if (player != null)
        {
            Player playerNode = player.GetComponent<Player>();
            if (playerNode != null && playerNode.PlayerBlanket != null)
            {
               playerNode.PlayerBlanket.enabled = GameManager.Instance.hasBlanketBase || GameManager.Instance.hasBlanketGlider;               
            }

            if (playerNode != null && playerNode.PlayerFlashlight != null)
            {
                bool flashOn = GameManager.Instance.hasFlashlight;
                playerNode.PlayerFlashlight.enabled = flashOn;

                if (!flashOn && playerNode.PlayerFlashlight.lightBeamObject != null)
                {
                    playerNode.PlayerFlashlight.lightBeamObject.SetActive(false);
                }
            }
        }
    }
}

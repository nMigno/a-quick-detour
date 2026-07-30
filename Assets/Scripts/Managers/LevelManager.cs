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
        PlacePlayer();
        SyncSceneStates();
    }

    private void Start()
    {
        if (GameManager.Instance != null)
            {
                GameManager.Instance.PlayMusic(levelMusic);
                ApplyLevelRestrictions();
            }
    }

    private void ApplyLevelRestrictions()
    {
        if (disableFlashlightOnStart)
        {
            GameManager.Instance.hasFlashlight = false;
        }

        if (disableBlanketOnStart)
        {
            GameManager.Instance.hasBlanketGlider = false;
            GameManager.Instance.hasBlanketBase = false;
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
                    player.transform.position = doorsInScene[i].transform.position;
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
    }
}

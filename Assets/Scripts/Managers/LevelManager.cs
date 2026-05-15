using Unity.VisualScripting;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject flashlightUpgrade;
    [SerializeField] private GameObject blanketShield;

    [Header("Collectables")]
    [SerializeField] private GameObject key;

    private void Start()
    {
        PlacePlayer();
        SyncSceneStates();
    }

    private void PlacePlayer()
    {
        if (!string.IsNullOrEmpty(GameManager.Instance.doorID))
        {
            Door[] doorsInScene = FindObjectsByType<Door>(FindObjectsSortMode.None);

            foreach(Door door in doorsInScene)
            {
                if (door.myDoorID == GameManager.Instance.doorID)
                {
                    player.transform.position = door.transform.position;
                    GameManager.Instance.doorID = "";

                    return;
                }
            }
            Debug.LogWarning("Door ID not found in scene");
        }

        Vector2 checkpointPos = GameManager.Instance.lastCheckpointPosition;
        if (checkpointPos != Vector2.zero)
        {
            player.transform.position = checkpointPos;
        }
    }

    private void SyncSceneStates()
    {
        if (GameManager.Instance.hasFlashlightUpgrade && flashlightUpgrade != null)
        {
            flashlightUpgrade.SetActive(false);
        }

        if (GameManager.Instance.hasBlanketShield && blanketShield != null)
        {
            blanketShield.SetActive(false);
        }   

        if (GameManager.Instance.hasKey && key != null)
        {
            key.SetActive(false);
        }
    }
}

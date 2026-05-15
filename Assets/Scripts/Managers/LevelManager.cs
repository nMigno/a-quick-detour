using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject flashlightUpgrade;
    [SerializeField] private GameObject blanketShield;

    private void Start()
    {
        PlacePlayerAtCheckpoint();
        SyncSceneStates();
    }

    private void PlacePlayerAtCheckpoint()
    {
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
    }
}

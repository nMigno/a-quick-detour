using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class IllusionWall : MonoBehaviour
{
    public string wallID;
    private bool isRevealed = false;

    private void Awake()
    {
        wallID = SceneManager.GetActiveScene().name + "_Wall_" + transform.position.ToString();
    }

    private void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.revealedIllusionWalls.Contains(wallID))
        {
            isRevealed = true;
            ApplyRevealedVisuals();
        }
    }

    public void Reveal()
    {
        if (isRevealed) return;

        isRevealed = true;

        GameManager.Instance.RegisterRevealedWall(wallID);
        Debug.Log("Illusion wall dissapeared!");

        ApplyRevealedVisuals();
    }

    private void ApplyRevealedVisuals()
    {
        gameObject.SetActive(false);
    }
}

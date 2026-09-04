using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class UnlockBlanket : MonoBehaviour
{
    [Header("Visual Settings")]
    [SerializeField] private Sprite sofaWithoutBlanket;

    [Header("Text Event Settings")]
    [Tooltip("Canvas prefab with TextPopup script")]
    [SerializeField] private GameObject textPopupPrefab;

    [Tooltip("Text to display")]
    [TextArea(2, 4)]
    [SerializeField] private string popupMessage = "Hey, my blanket!";

    private SpriteRenderer spriteRenderer;
    private bool unlocked = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!unlocked && collision.CompareTag("Player"))
        {
            UnlockBlanketUse(collision.gameObject);
        }
    }

    private void UnlockBlanketUse(GameObject playerObj)
    {
        unlocked = true;
        spriteRenderer.sprite = sofaWithoutBlanket;

        Player player = playerObj.GetComponent<Player>();
        player.PlayerBlanket.enabled = true;
        GameManager.Instance.UnlockBlanketGlider();
        GameManager.Instance.UnlockBlanketBase();

        GameObject popupInstance = Instantiate(textPopupPrefab);
        TextPopup popupScript = popupInstance.GetComponent<TextPopup>();
        popupScript.ShowText(popupMessage);
    }
}

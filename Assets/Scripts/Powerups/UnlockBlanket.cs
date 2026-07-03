using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class UnlockBlanket : MonoBehaviour
{
    [Header("Visual Settings")]
    [SerializeField] private Sprite sofaWithoutBlanket;

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
    }
}

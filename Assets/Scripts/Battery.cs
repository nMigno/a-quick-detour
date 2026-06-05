using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class Battery : MonoBehaviour
{
    [Header("Settings")]
    public float respawnTime = 5f;

    private SpriteRenderer spriteRenderer;
    private Collider2D col;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        player.PlayerFlashlight.RechargeBattery();

        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        spriteRenderer.enabled = false;
        col.enabled = false;

        yield return new WaitForSeconds(respawnTime);
        
        spriteRenderer.enabled = true;
        col.enabled = true;
    }
}

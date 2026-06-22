using UnityEngine;
using System.Collections;

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
        
        // TODO: Adaptar la lógica si se confirma la mecánica de "reparación" de la linterna rota.
        // El método original (RechargeBattery) fue eliminado al hacer la linterna infinita.
        // player.PlayerFlashlight.RechargeBattery(); 

        Debug.LogWarning("Batería recogida. La mecánica está suspendida a la espera de confirmación de Game Design.");

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
using System;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Tooltip("Active checkpoint color")]
    [SerializeField] private Color activeColor = Color.green;

    private SpriteRenderer spriteRenderer;
    private bool isActivated = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isActivated && collision.CompareTag("Player"))
        {
            ActivateCheckpoint(collision.transform);
        }
    }

    private void ActivateCheckpoint(Transform playerTransform)
    {
        isActivated = true;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = activeColor;

            playerTransform.GetComponent<PlayerHP>().SetRespawnPoint(transform.position);
            Debug.Log("New checkpoint set");
        }


    }
}

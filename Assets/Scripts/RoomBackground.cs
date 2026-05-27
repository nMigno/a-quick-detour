using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class RoomBackground : MonoBehaviour
{
    [Header("Background colors")]
    public Color darkBackground = new Color(0.1f, 0.1f, 0.3f, 1f); // Dark blue
    public Color brightBackground = new Color(1f, 0.9f, 0.7f, 1f); // Warm yellow

    private SpriteRenderer bgRenderer;

    private void Awake()
    {
        bgRenderer = GetComponent<SpriteRenderer>();
    }
}

using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class GreyBox : MonoBehaviour
{
    public Color color = new Color(0.6f, 0.6f, 0.6f, 1f);

    void Awake()
    {
        var sr = GetComponent<SpriteRenderer>();
        var tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, color);
        tex.Apply();
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1);
    }
}

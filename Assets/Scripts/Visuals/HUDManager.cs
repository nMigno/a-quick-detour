using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; private set; }

    [Header("HP UI")]
    [Tooltip("Drag HP images here")]
    public Image[] hpIcons;
    [Tooltip("Color placeholders")]
    public Color hpActiveColor = Color.red;
    public Color hpInactiveColor = Color.gray;

    [Header("Flashlight Battery UI")]
    public Image[] batteryIcons;
    public Color batteryActiveColor = Color.yellow;
    public Color batteryInactiveColor = Color.gray;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        SyncHUD();
    }

    private void SyncHUD()
    {
        if (GameManager.Instance == null) return;

        for (int i = 0; i < hpIcons.Length; i++)
        {
            if (i < GameManager.Instance.currentHP)
            {
                hpIcons[i].color = hpActiveColor;
            }
            else
            {
                hpIcons[i].color = hpInactiveColor;
            }
        }

        for (int i = 0; i < batteryIcons.Length; i++)
        {
            if (i < GameManager.Instance.currentFlashlightUses)
            {
                batteryIcons[i].color = batteryActiveColor;
            }
            else
            {
                batteryIcons[i].color = batteryInactiveColor;
            }
        }
    }
}

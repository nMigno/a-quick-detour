using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonSound : MonoBehaviour
{
    public enum SoundType
    {
        Confirm,
        Cancel
    }

    [Header("Sound Settings")]
    public SoundType soundType = SoundType.Confirm;

    private void Awake()
    {
        Button btn = GetComponent<Button>();
        if (btn != null) btn.onClick.AddListener(PlaySound);
    }

    private void PlaySound()
    {
        if (GameManager.Instance == null) return;

        if (soundType == SoundType.Confirm)
        {
            GameManager.Instance.PlayUIConfirmSound();
        }
        else if (soundType == SoundType.Cancel)
        {
            GameManager.Instance.PlayUICancelSound();
        }
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }
}

using UnityEngine;

public class Player : MonoBehaviour
{
    //Public references to player Scripts
    public PlayerController Controller { get; private set; }
    public PlayerHP Health { get; private set; }
    public Flashlight PlayerFlashlight { get; private set; }
    public Blanket PlayerBlanket { get; private set; }
    public PlayerAudio Audio { get; private set; }

    private void Awake()
    {
        // Get every component before start
        Controller = GetComponent<PlayerController>();
        Health = GetComponent<PlayerHP>();
        PlayerFlashlight = GetComponentInChildren<Flashlight>();
        PlayerBlanket = GetComponentInChildren<Blanket>();      
        Audio = GetComponent<PlayerAudio>();  
    }
}

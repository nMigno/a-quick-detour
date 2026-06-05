using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class VoidKill : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
       if (other.CompareTag("Player"))
        {
            Debug.Log("Void fall. Instakill");
            GameManager.Instance.RespawnPlayer();
        }
    }

}

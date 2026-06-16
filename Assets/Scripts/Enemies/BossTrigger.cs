using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class BossTrigger : MonoBehaviour
{
    [SerializeField] private FinalBoss bossToActivate;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && bossToActivate != null)
        {
            bossToActivate.ActivateBoss();
            gameObject.SetActive(false);
        }
    }
}

using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class Battery : MonoBehaviour
{
    [Header("Pickup Events")]
    public UnityEvent OnPickedUp;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            OnPickedUp?.Invoke();
            gameObject.SetActive(false);
        }
    }
}
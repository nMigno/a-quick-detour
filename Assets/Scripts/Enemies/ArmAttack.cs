using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class ArmAttack : MonoBehaviour
{
    [Header("Transform References")]
    [SerializeField] private Transform handTransform;
    [SerializeField] private Transform strikeTarget;

    [Header("Speeds and Times")]
    [SerializeField] private float strikeSpeed = 18f;
    [SerializeField] private float retractSpeed = 3f;
    [SerializeField] private float cooldownDuration = 2f;

    private Vector3 restPosition;
    private Coroutine attackRoutine;
    private bool playerInRange = false;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;

        if (handTransform != null) restPosition = handTransform.position;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (attackRoutine == null)
            {
                attackRoutine = StartCoroutine(AttackLoop());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    private IEnumerator AttackLoop()
    {
        while (playerInRange)
        {
            while (Vector3.Distance(handTransform.position, strikeTarget.position) > 0.01f)
            {
                handTransform.position = Vector3.MoveTowards(
                    handTransform.position,
                    strikeTarget.position,
                    strikeSpeed * Time.deltaTime
                );
                yield return null;
            }

            while (Vector3.Distance(handTransform.position, restPosition) > 0.01f)
            {
                handTransform.position = Vector3.MoveTowards(
                    handTransform.position,
                    restPosition,
                    retractSpeed * Time.deltaTime 
                );
                yield return null;
            }

            yield return new WaitForSeconds(cooldownDuration);
        }
        
        while (Vector3.Distance(handTransform.position, restPosition) > 0.01f)
        {
            handTransform.position = Vector3.MoveTowards(
                handTransform.position, 
                restPosition, 
                retractSpeed * Time.deltaTime
            );
            yield return null;
        }

        attackRoutine = null;        
    }
}

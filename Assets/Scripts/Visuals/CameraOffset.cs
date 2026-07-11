using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class CameraOffset : MonoBehaviour
{
    [Header("Cinemachine Settings")]
    [SerializeField] private CinemachineCamera targetCamera;

    [Header("Offset Settings")]
    [SerializeField] private float targetOffsetY = -4f;
    [SerializeField] private float transitionDuration = 1.5f;

    [Header("Behaviour")]
    [Tooltip("if true, camera goes back to original position")]
    [SerializeField] private bool revertOnExit = true;

    private CinemachinePositionComposer composer;
    private Vector3 defaultOffset;
    private Coroutine transitionCoroutine;

    private void Awake()
    {
        if (targetCamera != null)
        {
            composer = targetCamera.GetComponent<CinemachinePositionComposer>();
        }
    }

    private void Start()
    {
        if (composer != null)
        {
            defaultOffset = composer.TargetOffset;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && composer != null)
        {
            if (transitionCoroutine != null) StopCoroutine(transitionCoroutine);
            transitionCoroutine = StartCoroutine(TransitionOffset(targetOffsetY));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (revertOnExit && collision.CompareTag("Player") && composer != null)
        {
            if (transitionCoroutine != null) StopCoroutine(transitionCoroutine);
            transitionCoroutine = StartCoroutine(TransitionOffset(defaultOffset.y));
        }
    }

    private IEnumerator TransitionOffset(float targetY)
    {
        float elapsedTime = 0f;
        Vector3 startOffset = composer.TargetOffset;
        Vector3 endOffset = new Vector3(startOffset.x, targetY, startOffset.z);

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;

            float t = elapsedTime / transitionDuration;
            t = Mathf.SmoothStep(0f, 1f, t);

            composer.TargetOffset = Vector3.Lerp(startOffset, endOffset, t);

            yield return null;
        }

        composer.TargetOffset = endOffset;
    }
}

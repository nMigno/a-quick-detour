using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class CameraOffset : MonoBehaviour
{
    [Header("Cinemachine Settings")]
    [SerializeField] private CinemachineCamera targetCamera;

    [Header("Offset Settings")]
    [SerializeField] private bool modifyX = false;
    [SerializeField] private bool modifyY = true;
    [SerializeField] private float targetOffsetX = 0f;
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

            Vector3 target = composer.TargetOffset;
           if (modifyX) target.x = targetOffsetX;
           if (modifyY) target.y = targetOffsetY;

            transitionCoroutine = StartCoroutine(TransitionOffset(target));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (revertOnExit && collision.CompareTag("Player") && composer != null)
        {
            if (transitionCoroutine != null) StopCoroutine(transitionCoroutine);

            Vector3 target = composer.TargetOffset;
            if (modifyX) target.x = defaultOffset.x;
            if (modifyY) target.y = defaultOffset.y;

            transitionCoroutine = StartCoroutine(TransitionOffset(target));
        }
    }

    private IEnumerator TransitionOffset(Vector3 endOffset)
    {
        float elapsedTime = 0f;
        Vector3 startOffset = composer.TargetOffset;

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

using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Flashlight : MonoBehaviour
{
    // Flashlight states
    public enum FlashlightState { Ready, On, Jammed }
    public FlashlightState currentState = FlashlightState.Ready;

    [Header("Battery Settings")]
    public int maxUses = 2;

    [Header("Time (seconds)")]
    public float activeDuration = 1f;
    public float cooldownDuration = 3f;
    public float stunDuration = 3f; // Value for enemies

    [Header("Light Settings")]
    public float lightDistance = 5f;
    [Tooltip("Light thickness from flashlight")]
    public float baseThickness = 0.5f;
    [Tooltip("Light thickness at max distance")]
    public float maxThickness = 3f;
    [Tooltip("Light SortingLayer")]
    [SerializeField] string Layer;

    [Header("References")]
    public GameObject lightBeamObject; // Stun trigger
    public SpriteRenderer feedbackRenderer; // Flashlight visual
    
    private MeshFilter beamMeshFilter;
    private MeshRenderer beamMeshRenderer;

    [Header("Visual feedback")]
    public Color readyColor = Color.green;
    public Color onColor = Color.white;
    public Color jammedColor = Color.red;
    public Color beamColor = new Color (1f, 0.9f, 0.1f, 0.5f);
    public Color revealBeamColor = Color.orange;

    private Player playerNode;
    private PlayerInput playerInput;
    private InputAction flashlightAction;
    private InputAction revealAction;
    private PolygonCollider2D beamCollider;

    private void Awake()
    {
        playerNode = GetComponentInParent<Player>();
        playerInput = GetComponentInParent<PlayerInput>();
        flashlightAction = playerInput.actions.FindAction("Flashlight");
        revealAction = playerInput.actions.FindAction("RevealLight");

        if (lightBeamObject != null)
        {
            beamCollider = lightBeamObject.GetComponent<PolygonCollider2D>();
            beamMeshFilter = lightBeamObject.GetComponent<MeshFilter>();
            beamMeshRenderer = lightBeamObject.GetComponent<MeshRenderer>();

            LightBeam lbScript = lightBeamObject.GetComponent<LightBeam>();
            if (lbScript != null)
            {
                lbScript.flashlightController = this;
            }
        }
    }

    private void OnEnable()
    {
        if (flashlightAction != null) flashlightAction.performed += OnFlashlightInput;
        if (revealAction != null) revealAction.performed += OnRevealInput;
    }

    private void OnDisable()
    {
        if (flashlightAction != null) flashlightAction.performed -= OnFlashlightInput;
        if (revealAction != null) revealAction.performed -= OnRevealInput;
    }

    private void Start()
    {
        SetState(FlashlightState.Ready);
        lightBeamObject.SetActive(false);
        ConfigureLightBeam(beamColor);
    }

    private void ConfigureLightBeam(Color activeColor)
    {
        if (beamCollider != null)
        {
            // LightBeam Polygon vertices settings
            Vector2[] points = new Vector2[4];

            points[0] = new Vector2(0, baseThickness / 2f); // Up-left vertex
            points[1] = new Vector2(0, -baseThickness / 2f); // Down-left vertex
            points[2] = new Vector2(lightDistance, -maxThickness / 2f); // Down-right vertex
            points[3] = new Vector2(lightDistance, maxThickness / 2f); // Up-right vertex

            beamCollider.SetPath(0, points); // Assign points to polygon

            if (beamMeshFilter != null)
            {
                GenerateTrapezoidMesh(points);
            }

            if (beamMeshRenderer != null)
            {
                beamMeshRenderer.material.color = activeColor;
            }
        }
    }

    private void GenerateTrapezoidMesh(Vector2[] points)
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[4];
        for (int i = 0; i < 4; i++)
        {
            vertices[i] = new Vector3(points[i].x, points[i].y, 0f);
        }

        // Draw 2 triangles with vertices so as to form a trapeze (Unity is so silly)
        int[] triangles = new int[6]
        {
            0, 3, 2, //Upper triangle
            0, 2, 1  // Lower triangle   
        };

        Vector2[] uv = new Vector2[4]
        {
            new Vector2(0, 1),
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1)
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;

        beamMeshFilter.mesh = mesh;
        beamMeshRenderer.sortingLayerName = Layer;
    }

    private void OnFlashlightInput(InputAction.CallbackContext context)
    {
        if (currentState == FlashlightState.Ready)
        {
            if (playerNode != null) playerNode.Audio.PlayFlashlightOn();

            if (GameManager.Instance.currentFlashlightUses > 0)
            {
                StartCoroutine(UseLight(false));
            }
            else
            {
                Debug.Log("Flashlight out of battery!");
            }
        }
    }

    private void OnRevealInput(InputAction.CallbackContext context)
    {
        if (currentState == FlashlightState.Ready)
        {
            if (GameManager.Instance.hasFlashlightUpgrade)
            {
                if (playerNode != null) playerNode.Audio.PlayFlashlightOn();

                StartCoroutine(UseLight(true));
            }
            else
            {
                Debug.Log("Upgrade not unlocked yet!");
            }
        }
    }

    private IEnumerator UseLight(bool isRevealMode)
    {
        if (!isRevealMode) 
        {
            GameManager.Instance.currentFlashlightUses--;
        }

        SetState(FlashlightState.On);

        LightBeam beamScript = lightBeamObject.GetComponent<LightBeam>();
        if (beamScript != null)
        {
            beamScript.isRevealMode = isRevealMode;
        }

        Color currentColor = isRevealMode ? revealBeamColor : beamColor;
        ConfigureLightBeam(currentColor);

        lightBeamObject.SetActive(true);

        yield return new WaitForSeconds(activeDuration);

        SetState(FlashlightState.Jammed);
        lightBeamObject.SetActive(false);

        yield return new WaitForSeconds(cooldownDuration);

        SetState(FlashlightState.Ready);
    }

    private void SetState(FlashlightState newState)
    {
        bool isStateChanging = currentState != newState;

        currentState = newState;

        if (feedbackRenderer != null)
        {
            switch (currentState)
            {
                case FlashlightState.Ready:
                    feedbackRenderer.color = readyColor;
                    if (isStateChanging && playerNode != null) playerNode.Audio.PlayFlashlightReady();
                    break;
                case FlashlightState.On:
                    feedbackRenderer.color = onColor;
                    break;
                case FlashlightState.Jammed:
                    feedbackRenderer.color = jammedColor;
                    if (isStateChanging && playerNode != null) playerNode.Audio.PlayFlashlightJammed();
                    break;    
            } 
        }
    }

    public void RechargeBattery()
    {
        GameManager.Instance.currentFlashlightUses = maxUses;
        Debug.Log("Flashlight recharged!");
    }
}

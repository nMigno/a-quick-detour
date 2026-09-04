using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class RebindButtonUI : MonoBehaviour
{
    [Header("Rebind Configuration")]
    [Tooltip("Exact name of action in InputMap (e.g. Jump)")]
    [SerializeField] private string actionName;

    [Tooltip("Binding Index. If its a simple button, uses to be 0. If its composite (WASD), Up=1, Down=2, etc.")]
    [SerializeField] private int bindingIndex = 0;

    [Tooltip("Ignore mouse clicks?")]
    [SerializeField] private bool excludeMouse = true;

    [Header("References")]
    [Tooltip("TextMeshPro goes here")]
    [SerializeField] private TextMeshProUGUI buttonText;

    [Tooltip("Scene RebindManager")]
    [SerializeField] private RebindManager rebindManager;

    private void Awake()
    {
        // Si no se asignó el texto manual, intenta agarrarlo de los hijos
        if (buttonText == null) buttonText = GetComponentInChildren<TextMeshProUGUI>();

        // Asignamos automáticamente la función al hacer clic
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(OnRebindClicked);
    }

    private void Start()
    {
        // Si no se asignó el manager, lo busca
        if (rebindManager == null) rebindManager = FindAnyObjectByType<RebindManager>();

        // Mostrar tecla real (por si hay config guardada en PlayerPrefs)
        UpdateButtonText(); 
    }

    // Suscripcion al evento Restaurar cuando se activa el botón
    private void OnEnable()
    {
        RebindManager.OnBindingsRestored += UpdateButtonText;
        if (rebindManager != null) UpdateButtonText();
    }

    // Desuscripción al evento para evitar errores de memoria
    private void OnDisable()
    {
        RebindManager.OnBindingsRestored -= UpdateButtonText;
    }

    // Pedimos el string correcto al RebindManager
    public void UpdateButtonText()
    {
        if (rebindManager != null && buttonText != null)
        {
            string keyName = rebindManager.GetBindingName(actionName, bindingIndex);

            keyName = keyName.Replace("D-Pad ", "")
                             .Replace("DPAD ", "")
                             .Replace("Left Stick ", "")
                             .Replace("Right Stick ", "");

            buttonText.text = keyName.ToUpper();
        }
    }

    private void OnRebindClicked()
    {
        if (rebindManager != null)
        {
            // Llamamos al manager con toda la info configurada en el inspector
            rebindManager.StartRebind(actionName, bindingIndex, buttonText, excludeMouse);
        }
        else
        {
            Debug.LogError("RebindButtonUI: RebindManager is missing");
        }
    }
}

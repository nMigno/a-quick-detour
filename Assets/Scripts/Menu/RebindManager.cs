using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections.Generic;
using System;

public class RebindManager : MonoBehaviour
{
    [Header("Input Asset")]
    [Tooltip("Drag InputSystem file here")]
    [SerializeField] private InputActionAsset inputActions;

    [Header("UI Elements")]
    [Tooltip("Drag Rebind_Overlay Object here")]
    [SerializeField] private GameObject rebindOverlay; 
    [Tooltip("Drag 'Press any key' text Object here")]
    [SerializeField] private TextMeshProUGUI overlayText;

    // Diccionario temporal para guardar la operación de rebindeo y poder cancelarla si hace falta
    private InputActionRebindingExtensions.RebindingOperation rebindOperation;

    // Evento glogbal para avisar a la UI que se resetearon los controles
    public static event Action OnBindingsRestored;

    private void Start()
    {
        // Al arrancar, nos aseguramos de que el overlay esté apagado
        if (rebindOverlay != null) rebindOverlay.SetActive(false);

        // Cargar los binds guardados anteriormente
        LoadBindings();
    }

    /// <summary>
    /// Inicia el proceso de rebindeo para una acción específica.
    /// Esta función se llamará desde el evento OnClick de cada botón de la UI.
    /// </summary>
    /// <param name="actionName">El nombre de la acción en el Input System (ej: "Move", "Jump")</param>
    /// <param name="bindingIndex">El índice del binding dentro de la acción. 
    /// Por ejemplo, en "Move" (Composite WASD), W podría ser el índice 1, S el 2, etc.</param>
    /// <param name="buttonText">El componente de texto del botón para actualizarlo al terminar.</param>
    /// <param name="excludeMouse">Si es verdadero, ignorará los clics del mouse durante el rebindeo.</param>

    public void StartRebind(string actionName, int bindingIndex, TextMeshProUGUI buttonText, bool excludeMouse = true)
    {
        // Buscamos la acción dentro del ActionAsset
        InputAction actionToRebind = inputActions.FindAction(actionName);

        if (actionToRebind == null)
        {
            Debug.LogError($"RebindManager: '{actionName}' not found");
            return;
        }

        // Activamos el overlay para bloquear la UI y avisar al jugador
        if (rebindOverlay != null) rebindOverlay.SetActive(true);
        if (overlayText != null) overlayText.text = $"Press a key to assign '{actionName}'... ";

        // Deshabilitamos la acción temporalmente para poder rebindearla
        actionToRebind.Disable();

        // Iniciamos la operación de rebindeo
        rebindOperation = actionToRebind.PerformInteractiveRebinding(bindingIndex)
            // Cancelar el rebindeo si el jugador aprieta Escape
            .WithCancelingThrough("<Keyboard>/escape")
            // Qué pasa cuando termina exitosamente
            .OnComplete(operation =>
             {
                 // Limpiamos y terminamos la operación
                 operation.Dispose();

                 // Habilitamos la acción de nuevo
                 actionToRebind.Enable();

                 // Actualizamos el texto del botón con el nombre de la nueva tecla
                 UpdateUI(actionToRebind, bindingIndex, buttonText);

                 // Guardamos los cambios
                 SaveBindings();

                 // Apagamos el overlay
                 if (rebindOverlay != null) rebindOverlay.SetActive(false);
             })
             // Qué pasa si el usuario cancela (apreta Escape)
             .OnCancel(operation =>
             {
                operation.Dispose();
                actionToRebind.Enable();
                if (rebindOverlay != null) rebindOverlay.SetActive(false);
             });

        // Si excludeMouse es true, agregamos una regla para ignorar el mouse
        if (excludeMouse)
        {
            rebindOperation.WithControlsExcluding("Mouse");
        }

        // Escuchamos input
        rebindOperation.Start();
    }

    /// <summary>
    /// Actualiza el texto del botón con la tecla asignada.
    /// </summary>
    /// 
    private void UpdateUI(InputAction action, int bindingIndex, TextMeshProUGUI buttonText)
    {
        // Obtenemos el nombre "legible" de la tecla (ej: "W" en lugar de "<Keyboard>/w")
        string keyName = action.GetBindingDisplayString(bindingIndex, InputBinding.DisplayStringOptions.DontIncludeInteractions);
        if (buttonText != null) buttonText.text = keyName;
    }

    public string GetBindingName(string actionName, int bindingIndex)
    {
        InputAction action = inputActions.FindAction(actionName);
        if (action != null)
        {
            return action.GetBindingDisplayString(bindingIndex, InputBinding.DisplayStringOptions.DontIncludeInteractions);
        }
        return "Unbound";
    }

    /// <summary>
    /// Restaura todos los controles a sus valores por defecto en el Asset.
    /// </summary>
    public void RestoreDefaults()
    {
        inputActions.RemoveAllBindingOverrides();
        SaveBindings(); // Guardamos el estado limpio
        Debug.Log("Controls restored to default");

        OnBindingsRestored?.Invoke(); // Evento para actualizar los botones
    }

    // --- Persistencia (Guardar y Cargar) ---

    private void SaveBindings()
    {
        // El Input System permite serializar los overrides a JSON
        string rebinds = inputActions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("rebinds", rebinds);
        PlayerPrefs.Save();
    }

    private void LoadBindings()
    {
        if (PlayerPrefs.HasKey("rebinds"))
        {
            string rebinds = PlayerPrefs.GetString("rebinds");
            inputActions.LoadBindingOverridesFromJson(rebinds);
        }
    }
}

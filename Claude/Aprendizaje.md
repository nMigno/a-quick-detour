# Aprendizaje: Crear un juego 2D en Unity con MCP (CoderGamester)

Lecciones aprendidas en la primera sesión de setup del proyecto metroidvania.

---

## Setup del MCP

### ✅ Lo que funciona
- Registrar el servidor con `claude mcp add` desde la terminal, NO editando `settings.json` a mano:
  ```
  claude mcp add mcp-unity node "C:/Unity/[Proyecto]/Library/PackageCache/com.gamelovers.mcp-unity@[hash]/Server~/build/index.js"
  ```
- Este comando escribe en `~/.claude.json` y Claude Code lo toma correctamente.

### ❌ Error cometido
- Intenté escribir `mcpServers` en `.claude/settings.json` del proyecto — eso es para el CLI, no para la extensión de VSCode.
- El botón "Configure Claude Code" en Unity da error si el config no existe previamente. **Ignorarlo** — usar el comando de terminal directamente.

---

## Crear sprites placeholder (grey boxes)

### ❌ Enfoque que NO funciona
Crear un `MonoBehaviour` que genere sprites en `Awake()` con `Texture2D` + `Sprite.Create()`:
```csharp
// Esto solo funciona en Play mode, no en el editor
void Awake() {
    var tex = new Texture2D(1, 1);
    tex.SetPixel(0, 0, Color.gray);
    sr.sprite = Sprite.Create(tex, ...);
}
```
El sprite existe en runtime pero en el editor la escena se ve vacía. Confuso y poco útil.

### ❌ Enfoque que NO funciona
Crear el PNG con PowerShell y escribir el `.meta` a mano. La importación de Unity no lo reconoce bien porque el `.meta` tiene que tener formato exacto que varía por versión.

### ✅ Flujo correcto
1. Crear el sprite nativo de Unity via MCP:
   ```
   execute_menu_item("Assets/Create/2D/Sprites/Square")
   ```
   Esto crea `Assets/Sprites/Square.png` con el `.meta` correcto automáticamente.

2. Crear un **Editor script** en `Assets/Editor/` que cargue el sprite y lo asigne a todos los `SpriteRenderer`:
   ```csharp
   [MenuItem("Tools/Setup Grey Boxes")]
   static void SetupGreyBoxes() {
       var sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Square.png");
       foreach (var sr in Object.FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None)) {
           sr.sprite = sprite;
           sr.color = new Color(0.6f, 0.6f, 0.6f);
           EditorUtility.SetDirty(sr);
       }
       EditorSceneManager.MarkSceneDirty(...);
   }
   ```

3. Ejecutar via MCP:
   ```
   execute_menu_item("Tools/Setup Grey Boxes")
   ```

### ⚠️ Nota sobre menu items
Los menu items de Editor scripts a veces **no se registran inmediatamente** después de compilar. Si `execute_menu_item` falla con "no menu named X", hacer:
```
execute_menu_item("Assets/Refresh")
recompile_scripts()
```
y volver a intentar.

---

## Componentes via MCP

### ❌ BoxCollider2D queda con size 0.0001
Al agregar `BoxCollider2D` via `update_component` sin especificar size, Unity lo crea con tamaño casi cero. **Siempre especificar el size explícitamente:**
```json
{ "componentName": "BoxCollider2D", "componentData": { "size": { "x": 1, "y": 1 } } }
```

### ❌ Referencias a objetos (Transform, GameObject) no se pueden asignar via instanceId
Intenté asignar `groundCheck` (un Transform) al `PlayerController` pasando el instanceId:
```json
{ "groundCheck": -4720 }
```
No funciona. Unity no acepta referencias a objetos de esta forma via MCP.

**Solución:** auto-asignar en `Awake()` con `transform.Find()`:
```csharp
void Awake() {
    if (groundCheck == null)
        groundCheck = transform.Find("GroundCheck");
}
```

### ⚠️ set_transform usa coordenadas WORLD, no locales
Al posicionar un hijo con `set_transform`, la posición es en world space. Si el padre está en y=1 y quiero el hijo en y=-0.55 **local**, tengo que pasarle y=0.45 **world** (1 + (-0.55) = 0.45), o usar `"space": "local"`.

---

## Input System

### ❌ Error común: usar Input legacy con el nuevo Input System
Los proyectos 2D de Unity 2022+ usan el **New Input System** por defecto. Usar `Input.GetAxisRaw()` tira error en runtime.

### ✅ Forma correcta con New Input System
```csharp
using UnityEngine.InputSystem;

var kb = Keyboard.current;
float horizontal = 0f;
if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) horizontal = -1f;
if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) horizontal = 1f;

if (kb.spaceKey.wasPressedThisFrame && isGrounded)
    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
```

---

## Flujo recomendado para setup de escena

1. **Crear GameObjects en batch** con `batch_execute` — mucho más rápido que llamadas individuales.
2. **Crear scripts** escribiendo el `.cs` directamente al filesystem y luego llamar `recompile_scripts()`.
3. **Verificar errores** con `get_console_logs(logType: "error")` después de cada cambio importante.
4. **Guardar siempre** con `save_scene()` al final.
5. **Verificar colisiones**: el `BoxCollider2D` necesita size explícito. Verificar con `get_gameobject()` si hay dudas.

---

## Herramientas MCP más útiles para 2D

| Herramienta | Para qué sirve |
|---|---|
| `batch_execute` | Crear/modificar múltiples objetos en una sola llamada |
| `execute_menu_item` | Disparar cualquier menú de Unity (crear sprites, refrescar, etc.) |
| `update_component` | Agregar componentes y setear propiedades (ojo con referencias a objetos) |
| `get_gameobject` | Verificar el estado real de un objeto antes de modificarlo |
| `get_console_logs` | Diagnosticar errores sin salir de Claude |
| `recompile_scripts` | Forzar compilación después de crear/editar scripts |
| `save_scene` | Guardar la escena (no se guarda automáticamente) |

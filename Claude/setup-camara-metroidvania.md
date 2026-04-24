# Setup: Sistema de Cámara Metroidvania

Instrucciones para ejecutar via **CoderGamester mcp-unity**.  
Ejecutá cada paso en orden. Los scripts ya están en el proyecto.

---

## Paso 1 — Verificar la escena activa

```
load_scene("Assets/Scenes/SampleScene.unity")
```

Luego confirmá con:
```
get_scene_info()
```

---

## Paso 2 — Revisar jerarquía y encontrar el jugador

```
resource: unity://scenes-hierarchy
```

Buscá el GameObject que tiene el componente `PlayerController`. Anotá su nombre/path exacto (probablemente "Player" o similar).

---

## Paso 3 — Asignar tag "Player" al jugador

Reemplazá `<path-del-jugador>` con el path encontrado en el paso anterior:

```
update_gameobject(
  path: "<path-del-jugador>",
  tag: "Player"
)
```

---

## Paso 4 — Agregar CameraFollow a la Main Camera

```
update_component(
  gameObjectPath: "Main Camera",
  componentName: "CameraFollow",
  properties: {
    smoothTime: 0.2,
    deadZone: { x: 0.5, y: 0.3 },
    lookaheadDistance: 2.0,
    lookaheadSmoothTime: 0.4,
    hasBounds: false
  }
)
```

> `CameraFollow` detecta el Player automáticamente por tag. No hace falta asignarlo a mano.

---

## Paso 5 — Verificar GroundCheck del jugador

```
get_gameobject(path: "<path-del-jugador>")
```

Verificá que tenga un hijo llamado `"GroundCheck"`. Si no existe, crealo:

```
update_gameobject(
  path: "<path-del-jugador>/GroundCheck",
  createIfNotExists: true,
  localPosition: { x: 0, y: -0.5, z: 0 }
)
```

---

## Paso 6 — Crear cuartos (opcional, estilo Metroidvania)

Hacé esto por cada cuarto del mapa. Ajustá posición y tamaño según el nivel.

### Crear el contenedor de cuartos

```
update_gameobject(
  path: "Rooms",
  createIfNotExists: true
)
```

### Crear un cuarto

```
update_gameobject(
  path: "Rooms/Room_01",
  createIfNotExists: true,
  localPosition: { x: 0, y: 0, z: 0 }
)
```

### Agregar BoxCollider2D como trigger

```
update_component(
  gameObjectPath: "Rooms/Room_01",
  componentName: "BoxCollider2D",
  properties: {
    size: { x: 20, y: 12 },
    isTrigger: true
  }
)
```

> Ajustá `size` para que cubra el área del cuarto. En URP 2D, la cámara default cubre aprox. 19x10 unidades.

### Agregar CameraRoom

```
update_component(
  gameObjectPath: "Rooms/Room_01",
  componentName: "CameraRoom",
  properties: {
    autoComputeBounds: true
  }
)
```

Repetí los tres bloques anteriores para `Room_02`, `Room_03`, etc.

---

## Paso 7 — Guardar la escena

```
save_scene()
```

---

## Verificación final

Entrá en Play Mode y chequeá:
- La cámara sigue al jugador suavemente
- Al correr hacia un lado se nota un leve adelanto (lookahead)
- Si configuraste cuartos, la cámara no sale de los bordes del cuarto actual
- En Scene View (fuera de Play) los cuartos se ven como cajas cyan

---

## Referencia de scripts

| Script | Ubicación |
|---|---|
| `CameraFollow` | `Assets/Scripts/CameraFollow.cs` |
| `CameraRoom` | `Assets/Scripts/CameraRoom.cs` |
| `PlayerController` | `Assets/Scripts/PlayerController.cs` |

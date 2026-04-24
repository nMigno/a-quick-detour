# Setup: Sistema de Cámara Metroidvania

Este archivo contiene instrucciones para que un MCP con acceso a Unity Editor las ejecute directamente.

## Contexto del proyecto

- **Engine:** Unity 6000.3.9f1, URP 2D
- **Escena principal:** `Assets/Scenes/SampleScene.unity`
- **Scripts de cámara ya creados:**
  - `Assets/Scripts/CameraFollow.cs`
  - `Assets/Scripts/CameraRoom.cs`

---

## Tareas a ejecutar en Unity Editor via MCP

### 1. Asignar tag "Player" al jugador

Buscá el GameObject que tiene el componente `PlayerController` en la escena `SampleScene` y asignale el tag **"Player"** (crealo si no existe).

### 2. Agregar CameraFollow a la Main Camera

En la escena `SampleScene`:
- Seleccioná el GameObject **"Main Camera"**
- Agregale el componente `CameraFollow`
- Dejá todos los valores en default (se detecta el Player automáticamente por tag)
- Asegurate que `Has Bounds` esté **desactivado** por ahora

Valores default del componente:
```
Smooth Time: 0.2
Dead Zone: (0.5, 0.3)
Lookahead Distance: 2
Lookahead Smooth Time: 0.4
Has Bounds: false
```

### 3. Crear cuartos (CameraRoom) — opcional

Por cada área del mapa que quieras que la cámara quede confinada:

1. Creá un **GameObject vacío** llamado `Room_01`, `Room_02`, etc.
2. Agregale un **BoxCollider2D**
3. Ajustá el `Size` del BoxCollider2D para que cubra toda el área del cuarto
4. Marcá el BoxCollider2D como **Is Trigger: true** (el script lo hace automático, pero por las dudas)
5. Agregale el componente `CameraRoom`
6. Dejá `Auto Compute Bounds: true`
7. Agrupalos bajo un GameObject vacío llamado `Rooms` para mantener orden

### 4. Verificar Ground Check del jugador

El `PlayerController` busca un hijo llamado `"GroundCheck"`. Verificá que el GameObject del jugador tenga un hijo con ese nombre posicionado en los pies del personaje (Y negativo respecto al padre, por ejemplo `(0, -0.5, 0)`).

---

## Cómo funciona el sistema

- **Sin salas configuradas:** la cámara sigue al jugador por todo el mapa sin límites.
- **Con CameraRoom:** cuando el jugador entra al trigger de un cuarto, la cámara se limita a ese cuarto. Al pasar al siguiente cuarto, los bounds cambian automáticamente.
- **Lookahead:** la cámara se adelanta levemente en la dirección en que corre el jugador.
- **Dead zone:** la cámara no reacciona a movimientos menores a 0.5 unidades en X y 0.3 en Y, evitando micro-temblores.

---

## Verificación post-setup

Una vez configurado, en Play Mode:
- [ ] El jugador se mueve y la cámara lo sigue suavemente
- [ ] Al correr hacia un lado se nota el lookahead
- [ ] Si hay CameraRooms, la cámara no sale del cuarto actual
- [ ] En el editor (fuera de Play Mode) los bounds del cuarto se ven en color cyan en la Scene View

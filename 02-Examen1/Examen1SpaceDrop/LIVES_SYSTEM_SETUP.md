# Sistema de Vidas del Jugador

## Descripción General

El jugador ahora tiene múltiples vidas antes de que termine el juego. Por defecto, el jugador tiene **4 vidas** y puede recibir daño varias veces antes del Game Over final.

## Mecánica de Vidas

### Funcionamiento:

1. **Vidas Iniciales**: El jugador comienza con 4 vidas
2. **Pérdida de Vida**: Cada vez que un asteroide toca al jugador, pierde 1 vida
3. **Respawn**: Si aún tiene vidas, el jugador reaparece después de un breve delay
4. **Game Over**: Solo cuando las vidas llegan a 0, el juego termina completamente

### Proceso de Muerte y Respawn:

1. **Colisión**: Asteroide toca al jugador
2. **Explosión**: Se reproduce el efecto de explosión
3. **Destrucción**: El jugador actual se destruye
4. **Pérdida de Vida**: Se reduce una vida del contador
5. **Verificación**:
   - Si quedan vidas > 0: Respawn después de 2 segundos
   - Si vidas = 0: Game Over definitivo
6. **Recreación**: Se crea un nuevo jugador en la posición inicial

## Configuración en Unity

### En el GameManager Inspector:

#### **Sistema de Vidas:**

- **Max Lives**: Número máximo de vidas (por defecto: 4)
- **Current Lives**: Vidas actuales (se resetea automáticamente)
- **Respawn Delay**: Tiempo en segundos antes de reaparecer (por defecto: 2)

#### **UI Elements:**

- **Score Text**: Texto para mostrar la puntuación
- **Lives Text**: Texto para mostrar las vidas restantes (**NUEVO**)
- **Game Over UI**: Panel que aparece al terminar el juego

### Configuración Automática de UI:

El script `UISetup` ahora crea automáticamente:

1. **Texto de Puntuación** (esquina superior izquierda)
2. **Texto de Vidas** (esquina superior derecha) - **NUEVO**
3. **Panel de Game Over**

## Personalización

### Cambiar Número de Vidas:

```csharp
// En GameManager.cs, línea de configuración:
public int maxLives = 4;  // Cambia este número
```

### Cambiar Tiempo de Respawn:

```csharp
// En GameManager.cs:
public float respawnDelay = 2f;  // Cambia este tiempo (en segundos)
```

### Cambiar Posición de Respawn:

El jugador reaparece en su posición inicial. Si quieres cambiar esto:

1. Modifica la posición inicial del jugador en la escena
2. O edita el método `CreateNewPlayer()` en `GameManager.cs`

## Métodos Públicos Disponibles

### En GameManager:

```csharp
// Obtener vidas actuales
int vidasActuales = GameManager.Instance.GetCurrentLives();

// Obtener vidas máximas
int vidasMaximas = GameManager.Instance.GetMaxLives();

// Verificar si el jugador está en proceso de respawn
bool respawning = GameManager.Instance.IsPlayerRespawning();

// Agregar una vida extra (para power-ups futuros)
GameManager.Instance.AddLife();

// Forzar muerte del jugador
GameManager.Instance.PlayerDied();
```

## Información Técnica

### Sistema de Respawn:

- El sistema crea un "prefab" dinámico del jugador en tiempo de ejecución
- Se preservan todas las configuraciones del jugador original
- El nuevo jugador mantiene todas las propiedades (velocidad, sprites, etc.)

### Prevención de Conflictos:

- Solo se procesa una muerte a la vez (`isPlayerRespawning`)
- El Game Over solo ocurre cuando las vidas llegan exactamente a 0
- Los asteroides no pueden causar múltiples muertes simultáneas

### UI Automática:

- El texto de vidas se actualiza automáticamente
- Posición: esquina superior derecha
- Formato: "Vidas: X"
- Color: blanco, fuente del sistema

## Ejemplos de Configuración

### Juego Fácil:

- **Max Lives**: 6
- **Respawn Delay**: 1.5f

### Juego Normal:

- **Max Lives**: 4 (por defecto)
- **Respawn Delay**: 2f (por defecto)

### Juego Difícil:

- **Max Lives**: 2
- **Respawn Delay**: 3f

### Juego Extremo:

- **Max Lives**: 1
- **Respawn Delay**: 0f (muerte instantánea = Game Over)

## Compatibilidad

### Con Sistemas Existentes:

- ✅ **Compatible** con sistema de asteroides multi-tamaño
- ✅ **Compatible** con sistema de cristales
- ✅ **Compatible** con sistema de disparos
- ✅ **Mantiene** todas las funcionalidades anteriores

### Escalabilidad Futura:

- Fácil agregar power-ups que den vidas extra
- Soporte para diferentes tipos de daño
- Posibilidad de invulnerabilidad temporal
- Sistema de puntuación por vidas restantes

## Solución de Problemas

### "No aparece el texto de vidas":

- Verifica que el `UISetup` esté en la escena
- O asigna manualmente un Text al campo `Lives Text` del GameManager

### "El jugador no reaparece":

- Verifica que el jugador tenga el tag "Player"
- Asegúrate de que no haya errores en la consola durante el respawn

### "Las vidas no se actualizan":

- El campo `Lives Text` debe estar asignado en el GameManager
- Verifica que el texto tenga un componente Text válido

### "Respawn infinito":

- Revisa que `maxLives` sea un número positivo
- Verifica que el método `PlayerDied()` se llame correctamente

## Características Destacadas

- ✅ **Sistema Robusto**: Maneja errores y casos extremos
- ✅ **UI Automática**: Configuración automática del texto de vidas
- ✅ **Respawn Inteligente**: Recrea el jugador con todas sus propiedades
- ✅ **Configurable**: Vidas y tiempos ajustables desde Inspector
- ✅ **Extensible**: Fácil agregar funcionalidades relacionadas
- ✅ **Compatible**: Funciona con todos los sistemas existentes

El sistema añade una segunda oportunidad al jugador, haciendo el juego más amigable mientras mantiene el desafío.

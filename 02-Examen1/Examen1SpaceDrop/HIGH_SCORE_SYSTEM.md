# Sistema de Puntuación Máxima (High Score) - Space Drop

## ✨ Funcionalidades Implementadas

### 🏆 Puntuación Máxima Persistente

- **Almacenamiento permanente**: Se guarda usando Unity PlayerPrefs
- **Se mantiene entre sesiones**: No se pierde al cerrar el juego
- **Actualización automática**: Se actualiza cuando superas tu récord actual

### 📍 Ubicaciones de la UI

#### 1. **Menú Principal**

- Muestra el récord actual debajo del título del juego
- Color amarillo dorado para destacar
- Se actualiza automáticamente al regresar al menú

#### 2. **Durante el Juego**

- Aparece en la parte superior centro de la pantalla
- Color amarillo para diferenciar de la puntuación actual
- Se actualiza en tiempo real cuando estableces un nuevo récord

#### 3. **Pantalla de Game Over**

- Muestra el récord actual para comparar con tu puntuación final
- Color cian para destacar
- Te permite ver si lograste un nuevo récord

## 🔧 Funcionamiento Técnico

### Métodos Principales

#### En GameManager.cs:

```csharp
// Cargar puntuación máxima al iniciar
highScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);

// Verificar nuevo récord al sumar puntos
if (score > highScore) {
    highScore = score;
    SaveHighScore();
}

// Guardar en PlayerPrefs
private void SaveHighScore() {
    PlayerPrefs.SetInt(HIGH_SCORE_KEY, highScore);
    PlayerPrefs.Save();
}
```

#### En MenuManager.cs:

```csharp
// Actualizar UI en tiempo real
public void UpdateHighScore(int highScore) {
    if (highScoreText != null) {
        highScoreText.text = $"Récord: {highScore}";
    }
}
```

### 🎯 Integración Automática

1. **Al iniciar el juego**: Carga automáticamente el récord guardado
2. **Durante el juego**: Verifica y actualiza el récord con cada punto
3. **Al finalizar**: Muestra el récord en la pantalla de Game Over
4. **En el menú**: Siempre muestra el récord actualizado

## 🎨 Diseño Visual

### Colores por Contexto:

- **Menú Principal**: Amarillo dorado (#FFFF00) - Llamativo y elegante
- **Durante el Juego**: Amarillo (#FFFF00) - Visible pero no distrae
- **Game Over**: Cian (#00FFFF) - Contrasta con el rojo del Game Over

### Posicionamiento:

- **Menú Principal**: Centro, debajo del título
- **Gameplay**: Superior centro, debajo del botón de pausa
- **Game Over**: Entre la puntuación final y los botones

## 🚀 Características Avanzadas

### 1. **Notificación de Nuevo Récord**

```
Debug.Log("¡Nuevo récord! Puntuación máxima: " + highScore);
```

### 2. **Persistencia Robusta**

- Clave única: `"SpaceDrop_HighScore"`
- Valor por defecto: 0 si no existe dato guardado
- Guardado inmediato al establecer nuevo récord

### 3. **Integración Completa**

- Compatible con el sistema de UI existente
- No interfiere con la puntuación actual
- Se actualiza en todos los paneles automáticamente

## 🎮 Experiencia del Jugador

### Primera Vez:

1. Récord muestra "0" en todos los paneles
2. Cualquier puntuación se convierte en el primer récord
3. Se guarda automáticamente

### Siguientes Partidas:

1. Muestra tu mejor puntuación anterior
2. Te motiva a superar tu propio récord
3. Celebra cuando logras un nuevo récord

### Persistente:

- El récord se mantiene aunque cierres Unity
- Se mantiene aunque reinicies la computadora
- Solo se resetea si borras los PlayerPrefs

## 🛠️ Funciones Adicionales

### Métodos Públicos Disponibles:

```csharp
// Obtener puntuación máxima actual
int record = gameManager.GetHighScore();

// Reiniciar estadísticas (opcional para debug)
gameManager.ResetStats();
```

### Casos de Uso:

- **Desarrollo**: Usar `ResetStats()` para pruebas
- **Distribución**: El récord se mantiene entre actualizaciones del juego
- **Competencia**: Los jugadores pueden competir por el récord más alto

## ✅ Verificación

### Para Confirmar que Funciona:

1. **Ejecuta el juego** - Verifica que aparece "Récord: 0" en el menú
2. **Juega y consigue puntos** - Verifica que el récord se actualiza durante el juego
3. **Ve al Game Over** - Verifica que muestra tu nueva puntuación máxima
4. **Reinicia el juego** - Verifica que el récord se mantiene guardado
5. **Supera tu récord** - Verifica que se actualiza y muestra el mensaje en la consola

### Mensajes de Debug:

```
Puntuación máxima cargada: [número]
¡Nuevo récord! Puntuación máxima: [número]
Puntuación máxima guardada: [número]
```

---

**¡Tu juego ahora tiene un sistema completo de puntuación máxima que motivará a los jugadores a seguir jugando para superar sus propios récords!** 🏆✨

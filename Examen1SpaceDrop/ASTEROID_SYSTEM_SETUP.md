# Sistema de Asteroides Multi-Tamaño

## Descripción General

El sistema de asteroides ahora soporta 3 tamaños diferentes, cada uno con resistencia, tamaño y puntuación distintos. Los asteroides requieren múltiples impactos para ser destruidos según su tamaño.

## Tipos de Asteroides Disponibles

### 1. Asteroide Pequeño (Small)

- **Impactos necesarios**: 1
- **Valor de puntos**: 30
- **Probabilidad por defecto**: 50%
- **Escala**: 0.5x (50% del tamaño normal)
- **Descripción**: Fácil de destruir, da más puntos por ser rápido de eliminar

### 2. Asteroide Mediano (Medium)

- **Impactos necesarios**: 2
- **Valor de puntos**: 20
- **Probabilidad por defecto**: 35%
- **Escala**: 1.0x (tamaño normal)
- **Descripción**: Balance entre resistencia y puntos

### 3. Asteroide Grande (Large)

- **Impactos necesarios**: 3
- **Valor de puntos**: 10
- **Probabilidad por defecto**: 15%
- **Escala**: 1.5x (150% del tamaño normal)
- **Descripción**: Muy resistente, pero da menos puntos. Más difícil de destruir

## Mecánica de Daño

### Sistema de Salud

- Cada asteroide tiene **salud máxima** según su tamaño
- Al recibir un impacto de bala, **pierde 1 punto de salud**
- Cuando la salud llega a **0**, el asteroide es destruido

### Efecto Visual de Daño

- Cuando un asteroide recibe daño pero no es destruido, **parpadea en rojo** brevemente
- Esto indica al jugador que el asteroide fue impactado pero necesita más disparos

### Destrucción de Balas

- Las balas se destruyen **inmediatamente** al impactar cualquier asteroide
- Esto significa que cada bala solo puede dañar un asteroide una vez

## Configuración en Unity

### Método 1: Sprites Globales en el AsteroidSpawner (RECOMENDADO)

1. **Prepara los sprites** (3 sprites diferentes):

   - `asteroid_small.png` - Sprite pequeño
   - `asteroid_medium.png` - Sprite mediano
   - `asteroid_large.png` - Sprite grande

2. **Configura el AsteroidSpawner**:

   - Selecciona el GameObject que tiene el componente `AsteroidSpawner`
   - En "Configuración de Sprites de Asteroides (Opcional)":
     - **Small Asteroid Sprite**: Sprite para asteroides pequeños
     - **Medium Asteroid Sprite**: Sprite para asteroides medianos
     - **Large Asteroid Sprite**: Sprite para asteroides grandes

3. **Configura las probabilidades**:
   - En "Configuración de Tipos de Asteroides":
     - **Small Asteroid Chance**: Probabilidad de asteroides pequeños (0-100%)
     - **Medium Asteroid Chance**: Probabilidad de asteroides medianos (0-100%)
     - **Large Asteroid Chance**: Probabilidad de asteroides grandes (0-100%)

### Método 2: Prefabs Separados por Tamaño

1. **Crea prefabs separados**:

   - `SmallAsteroid` - Con sprite pequeño
   - `MediumAsteroid` - Con sprite mediano
   - `LargeAsteroid` - Con sprite grande

2. **Configura cada prefab**:
   - En el componente `AsteroidController` de cada prefab:
   - Configure el **Asteroid Size** por defecto
   - Asigna sprites específicos si lo deseas

## Personalización Avanzada

### Cambiar Valores de Puntos

Modifica el método `ConfigureAsteroidBySize()` en `AsteroidController.cs`:

```csharp
case AsteroidSize.Small:
    pointValue = 30;    // Cambia este valor
    break;
case AsteroidSize.Medium:
    pointValue = 20;    // Cambia este valor
    break;
case AsteroidSize.Large:
    pointValue = 10;    // Cambia este valor
    break;
```

### Cambiar Resistencia (Impactos Necesarios)

Modifica las líneas `maxHealth` en el mismo método:

```csharp
case AsteroidSize.Small:
    maxHealth = 1;      // Cambia para más/menos impactos
    break;
case AsteroidSize.Medium:
    maxHealth = 2;      // Cambia para más/menos impactos
    break;
case AsteroidSize.Large:
    maxHealth = 3;      // Cambia para más/menos impactos
    break;
```

### Cambiar Escalas/Tamaños

Modifica las variables públicas en el `AsteroidController`:

- `smallScale = 0.5f` // Tamaño del asteroide pequeño
- `mediumScale = 1.0f` // Tamaño del asteroide mediano
- `largeScale = 1.5f` // Tamaño del asteroide grande

## Configuraciones de Ejemplo

### Configuración Fácil (Más asteroides pequeños)

- Pequeños: 70%
- Medianos: 25%
- Grandes: 5%

### Configuración Balanceada (Por defecto)

- Pequeños: 50%
- Medianos: 35%
- Grandes: 15%

### Configuración Difícil (Más asteroides resistentes)

- Pequeños: 30%
- Medianos: 40%
- Grandes: 30%

### Configuración Extrema (Solo asteroides grandes)

- Pequeños: 10%
- Medianos: 20%
- Grandes: 70%

## Estrategia de Juego

### Para el Jugador:

- **Asteroides pequeños**: Prioridad alta - rápidos de destruir, buenos puntos
- **Asteroides medianos**: Prioridad media - balance costo/beneficio
- **Asteroides grandes**: Prioridad baja - consumen muchas balas, pocos puntos

### Balance del Juego:

- Los asteroides **más fáciles de destruir dan más puntos** (incentivo)
- Los asteroides **más resistentes ocupan más espacio** (peligro)
- Los jugadores deben **gestionar sus disparos** estratégicamente

## Información Técnica

### Sistema de Detección

- Cada bala puede impactar **solo una vez** antes de ser destruida
- Los asteroides **no colisionan entre sí**
- La salud se reduce en **1 punto por impacto**

### Efectos Visuales

- **Parpadeo rojo** al recibir daño sin ser destruido
- **Explosión** al ser destruido (si hay prefab asignado)
- **Escalado automático** según el tamaño del asteroide

### Nombres de Debug

- Los asteroides generados tienen nombres descriptivos: `Asteroid_[Tamaño]_[Número]`
- Ejemplo: `Asteroid_Large_5` = Asteroide grande número 5

## Compatibilidad

- **Compatible** con el sistema de cristales existente
- **Compatible** con el sistema de disparos del jugador
- **Mantiene** todas las funcionalidades anteriores de asteroides
- **Escalable** - fácil agregar más tipos de asteroides

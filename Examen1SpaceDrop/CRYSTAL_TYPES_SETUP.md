# Configuración de Tipos de Cristales

## Descripción General

El sistema de cristales ahora soporta múltiples tipos de cristales con diferentes valores de puntos y colores. Cada tipo tiene una probabilidad de aparición diferente para crear variedad en el juego.

## Tipos de Cristales Disponibles

### 1. Cristal Amarillo (Yellow)

- **Valor de puntos**: 50
- **Probabilidad por defecto**: 50%
- **Color**: Amarillo (Color.yellow)
- **Descripción**: Cristal común, fácil de encontrar

### 2. Cristal Azul (Blue)

- **Valor de puntos**: 75
- **Probabilidad por defecto**: 30%
- **Color**: Azul (Color.blue)
- **Descripción**: Cristal moderadamente raro, vale más puntos

### 3. Cristal Rojo (Red)

- **Valor de puntos**: 100
- **Probabilidad por defecto**: 15%
- **Color**: Rojo (Color.red)
- **Descripción**: Cristal raro, vale muchos puntos

### 4. Cristal Verde (Green)

- **Valor de puntos**: 150
- **Probabilidad por defecto**: 5%
- **Color**: Verde (Color.green)
- **Descripción**: Cristal muy raro, vale la máxima cantidad de puntos

## Configuración en Unity

### Método 1: Sprites Globales en el CrystalSpawner (RECOMENDADO)

Esta es la forma más fácil de configurar sprites diferentes para cada tipo de cristal:

1. **Prepara los sprites**:

   - Crea o importa 4 sprites diferentes para cada tipo de cristal
   - Por ejemplo: `crystal_yellow.png`, `crystal_blue.png`, `crystal_red.png`, `crystal_green.png`

2. **Configura el CrystalSpawner**:

   - Selecciona el GameObject que tiene el componente `CrystalSpawner`
   - En el Inspector, encontrarás la sección "Configuración de Sprites de Cristales (Opcional)"
   - Arrastra cada sprite correspondiente a su campo:
     - **Yellow Crystal Sprite**: Sprite para cristales amarillos
     - **Blue Crystal Sprite**: Sprite para cristales azules
     - **Red Crystal Sprite**: Sprite para cristales rojos
     - **Green Crystal Sprite**: Sprite para cristales verdes

3. **Configurar probabilidades**:

   - En la sección "Configuración de Tipos de Cristales", ajusta las probabilidades:
     - **Yellow Crystal Chance**: Probabilidad de cristales amarillos (0-100%)
     - **Blue Crystal Chance**: Probabilidad de cristales azules (0-100%)
     - **Red Crystal Chance**: Probabilidad de cristales rojos (0-100%)
     - **Green Crystal Chance**: Probabilidad de cristales verdes (0-100%)

4. **Configurar los Prefabs de Cristales**:
   - Puedes usar un solo prefab de cristal genérico
   - Asegúrate de que tenga:
     - Un componente `CrystalController`
     - Un `SpriteRenderer` (el sprite inicial no importa, se cambiará automáticamente)
     - Un `Collider2D` configurado como trigger
     - Un `Rigidbody2D` (se agregará automáticamente si no existe)

### Método 2: Sprites Individuales en cada Prefab

Si prefieres tener prefabs separados para cada tipo de cristal:

1. **Crea prefabs separados**:

   - Crea 4 prefabs diferentes: `YellowCrystal`, `BlueCrystal`, `RedCrystal`, `GreenCrystal`
   - Cada uno con su sprite correspondiente en el `SpriteRenderer`

2. **Configura cada prefab individualmente**:

   - En cada prefab, en el componente `CrystalController`:
   - Configura el tipo por defecto (`Crystal Type`)
   - Asigna los sprites específicos en "Configuración de sprites por tipo"

3. **Asignar prefabs al spawner**:
   - Arrastra todos los prefabs al array "Crystal Prefabs" del `CrystalSpawner`

### Paso 3: Asignar Prefabs al Spawner

1. En el componente `CrystalSpawner`, asigna los prefabs de cristales al array "Crystal Prefabs"
2. Si usas el Método 1, puedes usar el mismo prefab repetido 4 veces o solo 1 vez
3. Si usas el Método 2, arrastra cada prefab específico

## Personalización Avanzada

### Cambiar Valores de Puntos

Los valores de puntos se configuran automáticamente según el tipo, pero puedes modificarlos editando el método `ConfigureCrystalByType()` en `CrystalController.cs`:

```csharp
case CrystalType.Yellow:
    pointValue = 50;    // Cambia este valor
    break;
case CrystalType.Blue:
    pointValue = 75;    // Cambia este valor
    break;
// ... etc
```

### Cambiar Colores

Los colores se pueden personalizar en cada prefab de cristal:

1. Selecciona el prefab del cristal
2. En el componente `CrystalController`, modifica los colores en la sección "Configuración de colores"
3. O cambia el sprite del `SpriteRenderer` por uno de color diferente

### Cambiar Probabilidades Durante el Juego

Las probabilidades se normalizan automáticamente, por lo que puedes:

- Usar cualquier valor entre 0 y 100
- No necesitas que sumen exactamente 100%
- Cambiar las probabilidades en tiempo real modificando las variables públicas

## Notas Técnicas

### Sistema de Probabilidades

- Las probabilidades se normalizan automáticamente al inicio del juego
- Si todas las probabilidades son 0, se usan valores por defecto
- El sistema usa números aleatorios para determinar qué tipo de cristal crear

### Identificación de Tipos

- Cada cristal generado tiene un nombre descriptivo: `Crystal_[Tipo]_[Número]`
- Los tipos se pueden consultar usando `GetCrystalType()` en el `CrystalController`
- Los valores de puntos se pueden consultar usando `GetPointValue()`

### Compatibilidad

- El sistema es compatible con el código existente
- Los cristales mantienen todas sus funcionalidades anteriores
- Se puede expandir fácilmente para agregar más tipos de cristales

## Ejemplo de Configuración

Para un juego balanceado, puedes usar estas configuraciones:

**Configuración Fácil** (más cristales valiosos):

- Amarillo: 40%
- Azul: 35%
- Rojo: 20%
- Verde: 5%

**Configuración Difícil** (menos cristales valiosos):

- Amarillo: 70%
- Azul: 20%
- Rojo: 8%
- Verde: 2%

**Configuración Extrema** (solo cristales raros ocasionalmente):

- Amarillo: 85%
- Azul: 10%
- Rojo: 4%
- Verde: 1%

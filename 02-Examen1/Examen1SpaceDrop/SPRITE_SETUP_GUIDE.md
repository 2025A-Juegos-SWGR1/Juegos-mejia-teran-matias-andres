# Guía Rápida: Configuración de Sprites de Cristales

## Configuración Más Fácil (Recomendada)

### 1. Preparar los Sprites

- Crea o consigue 4 imágenes diferentes para los cristales
- Importa las imágenes a Unity (Assets/Sprites/)
- Asegúrate de que estén configuradas como "Sprite (2D and UI)"

### 2. Configurar en el CrystalSpawner

1. Selecciona el objeto que tiene el componente `CrystalSpawner`
2. En el Inspector, busca "Configuración de Sprites de Cristales"
3. Arrastra cada sprite a su campo correspondiente:
   - `Yellow Crystal Sprite` = Sprite amarillo/dorado
   - `Blue Crystal Sprite` = Sprite azul
   - `Red Crystal Sprite` = Sprite rojo
   - `Green Crystal Sprite` = Sprite verde

### 3. ¡Listo!

- Los cristales ahora usarán automáticamente el sprite correcto según su tipo
- Cada tipo mantiene sus puntos diferentes (amarillo=50, azul=75, rojo=100, verde=150)
- Las probabilidades se mantienen configurables

## ¿Qué Pasa si NO asigno sprites?

- Los cristales usarán el sprite del prefab original
- Se aplicará un color automáticamente (amarillo, azul, rojo, verde)
- Seguirá funcionando, pero todos tendrán el mismo sprite base

## ¿Dónde están estos campos?

En el Inspector del `CrystalSpawner`, verás estas secciones:

```
[Header("Configuración de Sprites de Cristales (Opcional)")]
├── Yellow Crystal Sprite
├── Blue Crystal Sprite
├── Red Crystal Sprite
└── Green Crystal Sprite
```

## Consejos

- **Usa sprites del mismo tamaño** para que se vean consistentes
- **Sprites claros y diferentes** para que el jugador pueda distinguirlos fácilmente
- **Puedes dejar campos vacíos** si quieres que algunos tipos usen solo color
- **Los sprites se aplicarán automáticamente** cuando se generen los cristales

## Ejemplo de Nombres de Archivos

```
Assets/Sprites/Crystals/
├── crystal_yellow.png    (para Yellow Crystal Sprite)
├── crystal_blue.png      (para Blue Crystal Sprite)
├── crystal_red.png       (para Red Crystal Sprite)
└── crystal_green.png     (para Green Crystal Sprite)
```

## Solución de Problemas

- **"No veo los campos de sprites"**: Asegúrate de estar viendo el `CrystalSpawner`, no el `CrystalController`
- **"Los sprites no cambian"**: Verifica que hayas asignado sprites en el CrystalSpawner, no en el prefab
- **"Solo veo colores"**: Es normal si no has asignado sprites - el sistema usará colores como fallback

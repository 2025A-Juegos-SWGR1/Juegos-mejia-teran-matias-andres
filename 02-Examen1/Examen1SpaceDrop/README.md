# Space Drop 🚀

### Un juego arcade de supervivencia espacial desarrollado en Unity

[![Unity Version](https://img.shields.io/badge/Unity-2022.3+-blue.svg)](https://unity3d.com/get-unity/download)
[![Platform](https://img.shields.io/badge/Platform-PC-lightgrey.svg)](https://github.com)
[![Genre](https://img.shields.io/badge/Genre-Arcade%20Shooter-orange.svg)](https://github.com)
[![Status](https://img.shields.io/badge/Status-Complete-brightgreen.svg)](https://github.com)

---

## 📖 Descripción

**Space Drop** es un juego arcade de shoot 'em up donde controlas una nave espacial que debe sobrevivir a una lluvia constante de asteroides mientras recolecta cristales valiosos. Con mecánicas clásicas modernizadas, sistema de vidas múltiples, y progresión de dificultad adaptativa, ofrece una experiencia adictiva y desafiante.

### 🎯 Características Principales

- ✅ **Sistema de vidas múltiples** (4 vidas con respawn automático)
- ✅ **Asteroides multi-tamaño** con diferentes resistencias (1-3 impactos)
- ✅ **Cristales de valor variable** con sistema de rareza (4 tipos)
- ✅ **UI completa** con menús, pausado, y navegación
- ✅ **Puntuación persistente** con high scores guardados
- ✅ **Audio integrado** (música de fondo y efectos de sonido)
- ✅ **Progresión de dificultad** que aumenta gradualmente
- ✅ **Arquitectura modular** y código escalable

---

## 🎮 Cómo Jugar

### Controles

| Acción         | Control            | Descripción                    |
| -------------- | ------------------ | ------------------------------ |
| **Movimiento** | `WASD` o `Flechas` | Mover la nave en 8 direcciones |
| **Disparar**   | `Espacio`          | Disparar proyectiles           |
| **Pausar**     | `ESC`              | Pausar/reanudar el juego       |
| **Menús**      | `Clic ratón`       | Navegar por botones            |

### Objetivo

🏆 **Obtén la puntuación más alta** sobreviviendo ondas de asteroides y recolectando cristales.

### Mecánicas de Juego

#### 🪨 Asteroides

- **Pequeños**: 1 impacto → 30 puntos (50% probabilidad)
- **Medianos**: 2 impactos → 20 puntos (35% probabilidad)
- **Grandes**: 3 impactos → 10 puntos (15% probabilidad)
- Los asteroides parpadean en rojo al recibir daño
- Colisionar con asteroides resta 1 vida

#### 💎 Cristales

- **Amarillo**: 50 pts (disparo) / 25 pts (contacto) - 50% probabilidad
- **Azul**: 75 pts (disparo) / 37 pts (contacto) - 30% probabilidad
- **Rojo**: 100 pts (disparo) / 50 pts (contacto) - 15% probabilidad
- **Verde**: 150 pts (disparo) / 75 pts (contacto) - 5% probabilidad

#### ❤️ Sistema de Vidas

- Comienzas con **4 vidas**
- Cada colisión con asteroide resta 1 vida
- Respawn automático si tienes vidas restantes (2 segundos)
- Game Over al agotar todas las vidas

---

## 🛠️ Instalación y Configuración

## Configuración Básica

1. **Crea una escena vacía** o usa la escena SampleScene existente.

2. **Configura la nave del jugador**:

   - Crea un objeto vacío y renómbralo a "Player"
   - Añade un SpriteRenderer y asigna el sprite de la nave
   - Añade un Rigidbody2D:
     - Desactiva gravedad (Gravity Scale = 0)
     - Tipo de cuerpo: Dynamic
   - Añade un Collider2D (Box o Circle) y marca "Is Trigger"
   - **Importante**: Asigna el tag "Player" al objeto
   - Añade el script PlayerController al objeto

3. **Configura el GameManager**:

   - Crea un objeto vacío y renómbralo a "GameManager"
   - Añade el script GameManager al objeto
   - Como alternativa, puedes añadir el script InitializeGame a cualquier objeto en la escena, y el GameManager se creará automáticamente

4. **Configura la UI (Interfaz de Usuario)** (ACTUALIZADO):

   - Crea un Canvas (GameObject > UI > Canvas)
   - Añade un Text para la puntuación (GameObject > UI > Text)
   - **NUEVO**: Añade un Text para las vidas (GameObject > UI > Text)
   - Ajusta los textos para que sean visibles (puntuación: esquina superior izquierda, vidas: esquina superior derecha)
   - Crea un panel de Game Over (puede estar desactivado inicialmente)
   - Arrastra el texto de puntuación al campo "Score Text" del GameManager
   - **NUEVO**: Arrastra el texto de vidas al campo "Lives Text" del GameManager
   - Arrastra el panel de Game Over al campo "Game Over UI" del GameManager
   - Como alternativa, puedes añadir el script UISetup a un objeto en la escena para crear automáticamente toda la UI básica

5. **Configura el generador de asteroides** (ACTUALIZADO):

   - Crea un objeto vacío y renómbralo a "AsteroidSpawner"
   - Añade el script AsteroidSpawner al objeto
   - Crea prefabs de asteroides con:
     - SpriteRenderer
     - Rigidbody2D (sin gravedad)
     - Collider2D marcado como "Is Trigger"
     - Script AsteroidController
     - **Importante**: Asigna el tag "Asteroid" a cada prefab
   - Arrastra los prefabs de asteroides al campo "Asteroid Prefabs" del AsteroidSpawner
   - **NUEVO**: Configura las probabilidades de tamaños de asteroides en el Inspector
   - **NUEVO**: Opcionalmente, asigna sprites diferentes para cada tamaño de asteroide

6. **Configura el sistema de disparos** (NUEVO):

   - Crea un prefab de bala con:
     - SpriteRenderer (sprite pequeño para la bala)
     - Rigidbody2D (sin gravedad)
     - Collider2D marcado como "Is Trigger"
     - Script BulletController
     - **Importante**: Asigna el tag "PlayerBullet" al prefab
   - Arrastra el prefab de bala al campo "Bullet Prefab" del PlayerController
   - Ajusta la velocidad de disparo ("Fire Rate") y otros parámetros según tu preferencia

7. **Configura el sistema de cristales** (NUEVO):
   - Crea un objeto vacío y renómbralo a "CrystalSpawner"
   - Añade el script CrystalSpawner al objeto
   - Crea prefabs de cristales con:
     - SpriteRenderer (sprite de cristal)
     - Rigidbody2D (sin gravedad)
     - Collider2D marcado como "Is Trigger"
     - Script CrystalController
     - **Importante**: Asigna el tag "Crystal" a cada prefab
   - Arrastra los prefabs de cristales al campo "Crystal Prefabs" del CrystalSpawner
   - Ajusta las probabilidades de cada tipo de cristal en el Inspector

## Configuración de Tags

Asegúrate de tener configurados los siguientes tags en tu proyecto (Edit > Project Settings > Tags and Layers):

- "Player" - Para la nave del jugador
- "Asteroid" - Para todos los asteroides
- "PlayerBullet" - Para las balas del jugador
- "Crystal" - Para todos los cristales

---

## 🏗️ Estructura del Proyecto

### 📁 Organización de Scripts

El proyecto está organizado en una estructura clara y modular:

```
Assets/Scripts/
├── 📁 Managers/           # Gestión del juego
│   ├── GameManager.cs     # Manager principal del juego
│   ├── MenuManager.cs     # Gestión de menús
│   └── GameStateManager.cs # Control de estados
│
├── 📁 Entities/           # Entidades del juego
│   ├── PlayerController.cs    # Control del jugador
│   ├── AsteroidController.cs  # Control de asteroides
│   ├── CrystalController.cs   # Control de cristales
│   └── BulletController.cs    # Control de proyectiles
│
├── 📁 Spawners/           # Sistemas de generación
│   ├── AsteroidSpawner.cs # Generador de asteroides
│   └── CrystalSpawner.cs  # Generador de cristales
│
├── 📁 UI/                 # Interfaz de usuario
│   ├── UISetup.cs         # Configuración de UI
│   ├── CrystalStatsUI.cs  # Estadísticas de cristales
│   └── GameUIInitializer.cs # Inicialización de UI
│
├── 📁 Initialization/     # Scripts de inicialización
│   ├── GameInitializer.cs  # Inicialización del juego
│   └── InitializeGame.cs   # Configuración inicial
│
└── 📁 Utils/              # Utilidades y herramientas
    └── BackgroundSetup.cs # Configuración del fondo
```

### 🔧 Componentes Principales

#### 🎮 Managers

- **GameManager**: Control principal del juego, puntuación, vidas, audio
- **MenuManager**: Navegación entre menús y pantallas
- **GameStateManager**: Control de estados del juego (jugando, pausado, game over)

#### 🚀 Entities

- **PlayerController**: Movimiento, disparos, colisiones del jugador
- **AsteroidController**: Comportamiento y resistencia de asteroides
- **CrystalController**: Tipos de cristales y valores de puntuación
- **BulletController**: Movimiento y colisiones de proyectiles

#### 🎯 Spawners

- **AsteroidSpawner**: Generación automática de asteroides con dificultad progresiva
- **CrystalSpawner**: Generación de cristales con probabilidades configurables

---

## Sistema de Asteroides Multi-Tamaño

El juego incluye 3 tipos diferentes de asteroides, cada uno con resistencia, tamaño y puntuación distintos:

### Tipos Disponibles:

- **Asteroide Pequeño**: 1 impacto para destruir, 30 puntos (50% probabilidad por defecto)
- **Asteroide Mediano**: 2 impactos para destruir, 20 puntos (35% probabilidad por defecto)
- **Asteroide Grande**: 3 impactos para destruir, 10 puntos (15% probabilidad por defecto)

### Mecánica de Daño:

- Los asteroides requieren **múltiples impactos** para ser destruidos
- Cada bala hace **1 punto de daño**
- Los asteroides **parpadean en rojo** cuando reciben daño sin ser destruidos
- Las balas se destruyen al impactar (cada bala puede dañar solo una vez)

### Personalización:

- Las probabilidades se pueden ajustar en el Inspector del AsteroidSpawner
- Los sprites se pueden configurar para cada tamaño de asteroide
- Los tamaños y escalas son configurables

Para más detalles, consulta el archivo `ASTEROID_SYSTEM_SETUP.md`.

## Sistema de Vidas del Jugador

El jugador ahora tiene múltiples vidas antes del Game Over final:

### Mecánica de Vidas:

- **Vidas Iniciales**: 4 vidas por defecto
- **Pérdida de Vida**: Cada colisión con asteroide resta 1 vida
- **Respawn**: Si quedan vidas, el jugador reaparece después de 2 segundos
- **Game Over**: Solo cuando las vidas llegan a 0

### Configuración:

- Las vidas se pueden ajustar en el Inspector del GameManager
- El tiempo de respawn es configurable
- UI automática muestra las vidas restantes en la esquina superior derecha

Para más detalles, consulta el archivo `LIVES_SYSTEM_SETUP.md`.

## Sistema de Tipos de Cristales

El juego incluye 4 tipos diferentes de cristales, cada uno con su propio valor de puntos y probabilidad de aparición:

### Tipos Disponibles:

- **Cristal Amarillo**: 50 puntos (50% probabilidad por defecto)
- **Cristal Azul**: 75 puntos (30% probabilidad por defecto)
- **Cristal Rojo**: 100 puntos (15% probabilidad por defecto)
- **Cristal Verde**: 150 puntos (5% probabilidad por defecto)

### Personalización:

- Las probabilidades se pueden ajustar en el Inspector del CrystalSpawner
- Los colores se aplican automáticamente a los cristales
- Los valores de puntos se asignan automáticamente según el tipo

Para más detalles, consulta el archivo `CRYSTAL_TYPES_SETUP.md`.

## Sistema de Estadísticas (Opcional)

Puedes agregar el script `CrystalStatsUI` para rastrear estadísticas de cristales:

- Cuenta cuántos cristales de cada tipo han sido destruidos
- Rastrea los puntos totales obtenidos por cristales
- Proporciona UI opcional para mostrar estas estadísticas

## Solución de Problemas Comunes

1. **"GameManager no encontrado"**:

   - Asegúrate de tener un objeto con el componente GameManager en la escena
   - O añade el script InitializeGame a cualquier objeto en la escena

2. **"No hay texto de puntuación asignado"**:

   - Asigna un Text UI al campo "Score Text" del GameManager
   - O añade el script UISetup a un objeto en la escena

3. **Los asteroides no aparecen o no son visibles**:

   - Verifica que los prefabs de asteroides tengan SpriteRenderer con sprites asignados
   - Asegúrate de que estén en la capa visible por la cámara
   - Verifica que el AsteroidSpawner tenga los prefabs asignados

4. **No se detectan colisiones**:

   - Asegúrate de que los objetos tienen Collider2D
   - Verifica que los Collider2D estén marcados como "Is Trigger"
   - Confirma que los tags ("Player", "Asteroid", "PlayerBullet", "Crystal") están correctamente asignados

5. **Los disparos no funcionan**:

   - Verifica que el prefab de bala tenga el tag "PlayerBullet"
   - Asegúrate de que el BulletController esté asignado al prefab
   - Confirma que el "Bullet Prefab" esté asignado en el PlayerController

6. **Los cristales no aparecen con colores diferentes**:
   - Los colores se aplican automáticamente por código
   - Si quieres sprites diferentes para cada tipo, puedes usar prefabs diferentes
   - Verifica que el CrystalController esté correctamente asignado

## Características del Juego

### Mecánicas Principales:

- **Movimiento**: Usa las flechas o WASD para mover la nave
- **Disparos**: Presiona ESPACIO para disparar balas
- **Asteroides Multi-Tamaño**: 3 tipos de asteroides con diferente resistencia (1-3 impactos)
- **Sistema de Vidas**: El jugador tiene 4 vidas antes del Game Over
- **Respawn**: El jugador reaparece automáticamente tras morir si tiene vidas
- **Colisiones**: Los asteroides destruyen la nave al tocarla (resta 1 vida)
- **Sistema de Daño**: Los asteroides requieren múltiples impactos y muestran efectos visuales
- **Puntuación**: Destruye asteroides y cristales para obtener puntos
- **Cristales**: Diferentes tipos de cristales dan diferentes puntos

### Sistemas Implementados:

- ✅ Gestión limpia de fondos (solo sprite personalizado)
- ✅ Sistema de disparos con balas
- ✅ Asteroides multi-tamaño con sistema de resistencia/daño
- ✅ Sistema de vidas del jugador con respawn automático
- ✅ Spawning inteligente de asteroides y cristales
- ✅ Múltiples tipos de cristales con diferentes valores
- ✅ Sistema de puntuación
- ✅ Detección de colisiones avanzada
- ✅ Efectos visuales de daño (parpadeo rojo)
- ✅ UI completa (puntuación, vidas, game over)
- ✅ Sistema de estadísticas opcional

## Extensiones Sugeridas

Una vez que el juego básico funcione, puedes expandirlo con:

1. **Efectos visuales**:

   - Partículas al destruir asteroides/cristales
   - Animaciones de explosión
   - Trails para las balas

2. **Audio**:

   - Sonidos de disparo
   - Sonidos de explosión
   - Música de fondo

3. **Power-ups adicionales**:

   - Disparos múltiples
   - Escudo temporal
   - Disparos más rápidos

4. **Mejoras de gameplay**:

   - Vidas múltiples
   - Niveles con diferentes dificultades
   - Jefe final

5. **UI avanzada**:
   - Menú principal
   - Tabla de puntuaciones
   - Opciones de configuración

## Archivos Principales del Proyecto

### Scripts Principales:

- `GameManager.cs` - Gestión general del juego
- `PlayerController.cs` - Controles y movimiento del jugador
- `AsteroidSpawner.cs` / `AsteroidController.cs` - Sistema de asteroides
- `CrystalSpawner.cs` / `CrystalController.cs` - Sistema de cristales
- `BulletController.cs` - Sistema de disparos
- `BackgroundSetup.cs` - Gestión del fondo del juego

### Scripts de Configuración:

- `InitializeGame.cs` - Inicialización automática
- `UISetup.cs` - Configuración automática de UI
- `GameInitializer.cs` - Inicialización general del juego

### Scripts Opcionales:

- `CrystalStatsUI.cs` - Sistema de estadísticas de cristales

### Archivos de Documentación:

- `README.md` - Este archivo
- `CRYSTAL_TYPES_SETUP.md` - Guía detallada del sistema de cristales

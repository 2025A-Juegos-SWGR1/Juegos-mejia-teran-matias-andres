# Instrucciones para configurar el juego Space Drop

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

4. **Configura la UI (Interfaz de Usuario)**:

   - Crea un Canvas (GameObject > UI > Canvas)
   - Añade un Text para la puntuación (GameObject > UI > Text)
   - Ajusta el Text para que sea visible (esquina superior izquierda recomendada)
   - Crea un panel de Game Over (puede estar desactivado inicialmente)
   - Arrastra el texto de puntuación al campo "Score Text" del GameManager
   - Arrastra el panel de Game Over al campo "Game Over UI" del GameManager
   - Como alternativa, puedes añadir el script UISetup a un objeto en la escena para crear automáticamente la UI básica

5. **Configura el generador de asteroides**:
   - Crea un objeto vacío y renómbralo a "AsteroidSpawner"
   - Añade el script AsteroidSpawner al objeto
   - Crea prefabs de asteroides con:
     - SpriteRenderer
     - Rigidbody2D (sin gravedad)
     - Collider2D marcado como "Is Trigger"
     - Script AsteroidController
     - **Importante**: Asigna el tag "Asteroid" a cada prefab
   - Arrastra los prefabs de asteroides al campo "Asteroid Prefabs" del AsteroidSpawner

## Configuración de Tags

Asegúrate de tener configurados los siguientes tags en tu proyecto (Edit > Project Settings > Tags and Layers):

- "Player" - Para la nave del jugador
- "Asteroid" - Para todos los asteroides
- "PlayerBullet" - Si implementas disparos desde la nave

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
   - Confirma que los tags ("Player", "Asteroid") están correctamente asignados

## Extensiones Sugeridas

Una vez que el juego básico funcione, puedes expandirlo con:

1. Disparos desde la nave
2. Diferentes tipos de asteroides
3. Power-ups
4. Efectos visuales y sonoros
5. Menú principal y sistema de puntuación persistente

using UnityEngine;
using System.Linq; // Asegurarse de incluir LINQ para el uso de Where()

public class BackgroundSetup : MonoBehaviour
{
    [Header("Configuración del Fondo Personalizado")]
    public Sprite customBackgroundSprite;  // Asigna aquí tu sprite de fondo personalizado

    // Singleton pattern para asegurar que solo exista una instancia
    public static BackgroundSetup Instance { get; private set; }
    void Awake()
    {
        Debug.Log("BackgroundSetup: Awake iniciado");        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("BackgroundSetup: Instancia singleton creada");

            // Eliminar fondos no deseados antes de crear el nuestro - hacer esto de forma más agresiva
            RemoveUnwantedBackgrounds();

            // Esperar un frame antes de crear nuestro fondo para asegurar que los objetos marcados para destrucción hayan sido eliminados
            StartCoroutine(SetupBackgroundNextFrame());
        }
        else if (Instance != this)
        {
            Debug.Log("BackgroundSetup: Ya existe una instancia. Destruyendo duplicado.");
            Destroy(gameObject);
            return;
        }
    }

    private System.Collections.IEnumerator SetupBackgroundNextFrame()
    {
        yield return null; // Esperar un frame

        // Eliminar fondos no deseados otra vez por si acaso
        RemoveUnwantedBackgrounds();

        // Crear el fondo personalizado
        SetupCustomBackground();
    }

    void SetupCustomBackground()
    {
        Debug.Log("BackgroundSetup: Iniciando configuración de fondo personalizado...");

        // Verificar si ya existe un fondo
        GameObject existingBackground = GameObject.Find("CustomBackground");
        if (existingBackground != null)
        {
            Debug.Log("BackgroundSetup: Fondo personalizado ya existe. Se actualizará en lugar de crear uno nuevo.");
            // Eliminar el fondo existente para crear uno nuevo con las configuraciones actualizadas
            Destroy(existingBackground);
        }

        // Verificar que tengamos un sprite asignado
        if (customBackgroundSprite == null)
        {
            Debug.LogWarning("BackgroundSetup: No se ha asignado un sprite para el fondo personalizado.");
            return;
        }

        Debug.Log("BackgroundSetup: Sprite de fondo encontrado: " + customBackgroundSprite.name);

        try
        {
            // Crear un nuevo objeto para el fondo
            GameObject backgroundObj = new GameObject("CustomBackground");
            // Usamos una posición Z grande para asegurar que esté detrás de todo, pero dentro del rango de la cámara
            backgroundObj.transform.position = new Vector3(0, 0, 10);

            // Asignar layer al objeto para asegurar que se renderice correctamente
            backgroundObj.layer = LayerMask.NameToLayer("Default");

            // Intentar agregar el tag "Background" si es posible para facilitar la búsqueda
            try
            {
                backgroundObj.tag = "Background";
            }
            catch (System.Exception)
            {
                Debug.LogWarning("BackgroundSetup: No se pudo asignar el tag 'Background'. No es crítico.");
            }

            Debug.Log("BackgroundSetup: Objeto de fondo creado en posición: " + backgroundObj.transform.position);

            // Añadir un SpriteRenderer
            SpriteRenderer renderer = backgroundObj.AddComponent<SpriteRenderer>();
            renderer.sprite = customBackgroundSprite;

            // Configurar el orden de renderizado para asegurar visibilidad
            renderer.sortingLayerName = "Default";
            renderer.sortingOrder = -100; // Un valor muy bajo para asegurar que está detrás de todo

            // Asegurarse de que el SpriteRenderer esté configurado para ser visible
            renderer.enabled = true;

            Debug.Log("BackgroundSetup: SpriteRenderer configurado con sorting order: " + renderer.sortingOrder);

            // Verificar que el sprite tenga textura
            if (customBackgroundSprite.texture == null)
            {
                Debug.LogError("BackgroundSetup: ¡El sprite no tiene textura asignada!");
                return;
            }

            // Configurar el tamaño para cubrir toda la pantalla
            if (Camera.main == null)
            {
                Debug.LogError("BackgroundSetup: No se encontró la cámara principal (Main Camera)");
                return;
            }

            float height = Camera.main.orthographicSize * 2.1f; // Usamos 2.1 en lugar de 2.0 para dar un pequeño margen
            float width = height * Camera.main.aspect;

            Debug.Log("BackgroundSetup: Dimensiones de pantalla calculadas: " + width + " x " + height);

            // Aplicar escala para cubrir la pantalla
            // Ajustamos la escala para mantener la proporción del sprite original
            float spriteWidth = customBackgroundSprite.bounds.size.x;
            float spriteHeight = customBackgroundSprite.bounds.size.y;

            // Verificar que el sprite tenga dimensiones válidas
            if (spriteWidth <= 0 || spriteHeight <= 0)
            {
                Debug.LogError("BackgroundSetup: El sprite tiene dimensiones inválidas: " + spriteWidth + " x " + spriteHeight);
                // Usar valores por defecto en caso de error
                spriteWidth = spriteWidth <= 0 ? 1 : spriteWidth;
                spriteHeight = spriteHeight <= 0 ? 1 : spriteHeight;
            }

            float scaleX = width / spriteWidth;
            float scaleY = height / spriteHeight;

            Debug.Log("BackgroundSetup: Dimensiones del sprite: " + spriteWidth + " x " + spriteHeight);
            Debug.Log("BackgroundSetup: Escalas calculadas: X=" + scaleX + ", Y=" + scaleY);

            // Usamos la escala mayor para asegurar que cubra toda la pantalla
            float scale = Mathf.Max(scaleX, scaleY) * 1.1f; // Multiplicamos por 1.1 para dar un margen adicional
            backgroundObj.transform.localScale = new Vector3(scale, scale, 1);

            Debug.Log("BackgroundSetup: Escala final aplicada: " + scale);

            // Asegurarse de que el color tenga alfa 1 (completamente opaco)
            Color color = renderer.color;
            color.a = 1.0f;
            renderer.color = color;

            // Asegurarse de que el shader sea compatible con 2D
            Material material = new Material(Shader.Find("Sprites/Default"));
            if (material != null)
            {
                renderer.material = material;
                Debug.Log("BackgroundSetup: Material configurado con shader: " + material.shader.name);
            }
            else
            {
                Debug.LogWarning("BackgroundSetup: No se pudo encontrar el shader 'Sprites/Default'. Usando material por defecto.");
            }

            // Hacer que el objeto persista entre escenas
            DontDestroyOnLoad(backgroundObj);

            Debug.Log("BackgroundSetup: Fondo personalizado creado correctamente utilizando el sprite: " + customBackgroundSprite.name);

            // Verificación final del estado del objeto
            Debug.Log("BackgroundSetup: Estado final del SpriteRenderer - " +
                "Enabled: " + renderer.enabled +
                ", Visible: " + renderer.isVisible +
                ", Sprite: " + (renderer.sprite != null ? renderer.sprite.name : "null") +
                ", Material: " + (renderer.material != null ? renderer.material.name : "null") +
                ", Z Position: " + backgroundObj.transform.position.z);

            // Forzar un repintado/actualización del SpriteRenderer
            renderer.enabled = false;
            renderer.enabled = true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("BackgroundSetup: Error al configurar el fondo personalizado: " + e.Message);
        }
    }    // Método para forzar la actualización y visibilidad del fondo
    public void ForceBackgroundUpdate()
    {
        Debug.Log("BackgroundSetup: Forzando actualización del fondo...");

        try
        {
            // Eliminar fondos no deseados primero
            RemoveUnwantedBackgrounds();

            // Buscar todos los fondos existentes por nombre
            GameObject existingBackground = GameObject.Find("CustomBackground");
            if (existingBackground != null)
            {
                Debug.Log("BackgroundSetup: Destruyendo fondo existente: " + existingBackground.name);
                Destroy(existingBackground);
            }

            // Buscar por tag como alternativa
            try
            {
                GameObject[] taggedBackgrounds = GameObject.FindGameObjectsWithTag("Background");
                if (taggedBackgrounds != null && taggedBackgrounds.Length > 0)
                {
                    foreach (GameObject bg in taggedBackgrounds)
                    {
                        Debug.Log("BackgroundSetup: Destruyendo fondo con tag Background: " + bg.name);
                        Destroy(bg);
                    }
                }
            }
            catch (System.Exception)
            {
                Debug.LogWarning("BackgroundSetup: No se pudo buscar objetos con tag 'Background'. Posiblemente el tag no existe.");
            }

            // Asegurarse de que existe la cámara principal antes de continuar
            if (Camera.main == null)
            {
                Debug.LogError("BackgroundSetup: No hay cámara principal. Asegúrate de que existe una cámara con el tag 'MainCamera'.");
                return;
            }

            // Forzar la creación de un nuevo fondo
            SetupCustomBackground();

            Debug.Log("BackgroundSetup: Actualización de fondo completada.");
        }
        catch (System.Exception e)
        {
            Debug.LogError("BackgroundSetup: Error al forzar la actualización del fondo: " + e.Message);
        }
    }    // Método para buscar y eliminar fondos verdes u otros fondos no deseados
    private void RemoveUnwantedBackgrounds()
    {
        Debug.Log("BackgroundSetup: Buscando y eliminando fondos no deseados...");

        try
        {
            // Buscar específicamente objetos SpawnerMarker y eliminarlos
            GameObject[] spawnerMarkers = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None)
                .Where(obj => obj.name == "SpawnerMarker").ToArray();

            foreach (GameObject marker in spawnerMarkers)
            {
                Debug.Log("BackgroundSetup: Encontrado y eliminando SpawnerMarker: " + marker.name);
                Destroy(marker);
            }

            // Continuar con la búsqueda normal de fondos no deseados
            // Buscar todos los GameObjects en la escena que podrían ser fondos
            GameObject[] allObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            int removedCount = 0;

            foreach (GameObject obj in allObjects)
            {
                // Ignorar nuestro propio objeto
                if (obj == gameObject || obj.name == "CustomBackground")
                    continue;

                bool shouldRemove = false;

                // 1. Verificar por nombre si podría ser un fondo
                if (obj.name.Contains("Background") ||
                    obj.name.Contains("backdrop") ||
                    obj.name.Contains("_bg") ||
                    obj.name.Contains("Plane"))
                {
                    Debug.Log("BackgroundSetup: Encontrado posible fondo por nombre: " + obj.name);
                    shouldRemove = true;
                }

                // 2. Verificar si tiene un SpriteRenderer con características de fondo
                SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    // Si tiene sorting order muy bajo (típico de fondos)
                    if (sr.sortingOrder <= -10)
                    {
                        Debug.Log("BackgroundSetup: Encontrado SpriteRenderer con sorting order bajo: " + obj.name + " (" + sr.sortingOrder + ")");
                        shouldRemove = true;
                    }
                    // Si tiene color verde dominante (fondos de prueba Unity)
                    Color color = sr.color;
                    if (color.g > color.r || color.g > color.b) // Menos restrictivo para detectar cualquier tinte verde
                    {
                        Debug.Log("BackgroundSetup: Encontrado SpriteRenderer con color verde: " + obj.name + ", Color: " + color);
                        shouldRemove = true;
                    }

                    // Si no tiene sprite pero es visible
                    if (sr.sprite == null && sr.enabled)
                    {
                        Debug.Log("BackgroundSetup: Encontrado SpriteRenderer sin sprite: " + obj.name);
                        shouldRemove = true;
                    }
                }

                // 3. Verificar si tiene un Renderer que no sea SpriteRenderer y está en una posición de fondo
                Renderer renderer = obj.GetComponent<Renderer>();
                if (renderer != null && !(renderer is SpriteRenderer))
                {
                    // Si está posicionado detrás (Z positivo grande en un juego 2D)
                    if (obj.transform.position.z > 0)
                    {
                        Debug.Log("BackgroundSetup: Encontrado Renderer en posición de fondo (Z = " + obj.transform.position.z + "): " + obj.name);
                        shouldRemove = true;
                    }
                }

                // 4. Verificar por escala muy grande (típico de fondos que cubren toda la pantalla)
                if (obj.transform.localScale.x > 10 && obj.transform.localScale.y > 10)
                {
                    Debug.Log("BackgroundSetup: Encontrado objeto con escala muy grande: " + obj.name);
                    shouldRemove = true;
                }

                // 5. Excluir objetos importantes que no deben eliminarse
                if (obj.name.Contains("Manager") ||
                    obj.name.Contains("Camera") ||
                    obj.name.Contains("Player") ||
                    obj.name.Contains("Canvas") ||
                    obj.name.Contains("UI") ||
                    obj.GetComponent<Camera>() != null)
                {
                    shouldRemove = false;
                }

                // Si debe eliminarse, hacerlo
                if (shouldRemove)
                {
                    Debug.Log("BackgroundSetup: Eliminando fondo no deseado: " + obj.name);
                    Destroy(obj);
                    removedCount++;
                }
            }

            Debug.Log("BackgroundSetup: Eliminados " + removedCount + " objetos de fondo no deseados");
        }
        catch (System.Exception e)
        {
            Debug.LogError("BackgroundSetup: Error al buscar fondos no deseados: " + e.Message);
        }
    }
}

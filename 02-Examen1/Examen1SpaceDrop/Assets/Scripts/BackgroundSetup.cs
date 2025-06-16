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
            Instance = this; DontDestroyOnLoad(gameObject);
            Debug.Log("BackgroundSetup: Instancia singleton creada");

            // Esperar un frame antes de crear nuestro fondo
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
            backgroundObj.layer = LayerMask.NameToLayer("Default");            // Intentar agregar el tag "Background" si es posible para facilitar la búsqueda
            try
            {
                // Verificar si el tag existe antes de asignarlo
                if (IsTagDefined("Background"))
                {
                    backgroundObj.tag = "Background";
                    Debug.Log("BackgroundSetup: Tag 'Background' asignado exitosamente.");
                }
                else
                {
                    Debug.LogWarning("BackgroundSetup: El tag 'Background' no está definido en el proyecto. Usando 'Untagged'.");
                    backgroundObj.tag = "Untagged";
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("BackgroundSetup: No se pudo asignar el tag 'Background'. Error: " + ex.Message + ". Usando 'Untagged'.");
                backgroundObj.tag = "Untagged";
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
            renderer.enabled = false; renderer.enabled = true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("BackgroundSetup: Error al configurar el fondo personalizado: " + e.Message);
        }
    }

    // Método para forzar la actualización y visibilidad del fondo
    public void ForceBackgroundUpdate()
    {
        Debug.Log("BackgroundSetup: Forzando actualización del fondo...");

        try
        {
            // Asegurarse de que existe la cámara principal antes de continuar
            if (Camera.main == null)
            {
                Debug.LogError("BackgroundSetup: No hay cámara principal. Asegúrate de que existe una cámara con el tag 'MainCamera'.");
                return;
            }

            // Forzar la creación de un nuevo fondo
            SetupCustomBackground();            Debug.Log("BackgroundSetup: Actualización de fondo completada.");
        }
        catch (System.Exception e)
        {
            Debug.LogError("BackgroundSetup: Error al forzar la actualización del fondo: " + e.Message);
        }
    }

    // Método auxiliar para verificar si un tag está definido
    private bool IsTagDefined(string tagName)
    {
        try
        {
            // Intentar crear un array de objetos con el tag. Si el tag no existe, se lanza una excepción
            GameObject.FindGameObjectsWithTag(tagName);
            return true;
        }
        catch (UnityEngine.UnityException)
        {
            return false;
        }
    }
}

using UnityEngine;
using System.Linq; // Añadido para usar LINQ en la búsqueda de objetos

// Este script debe añadirse a un GameObject en la escena para asegurar la correcta inicialización del juego
public class GameInitializer : MonoBehaviour
{
    // Referencia al prefab de GameManager (opcional)
    public GameManager gameManagerPrefab;

    // Referencia a UISetup (opcional)
    public UISetup uiSetupPrefab;

    // Referencia a BackgroundSetup (opcional)
    public BackgroundSetup backgroundSetupPrefab;
    public Sprite customBackgroundSprite; // Tu propio asset de fondo

    [Header("Sonidos del juego (opcional)")]
    public AudioClip gameOverSound;
    public AudioClip scoreSound;

    [Header("Componentes de UI (opcional)")]
    public Canvas mainCanvas;
    public UnityEngine.UI.Text scoreTextPrefab;
    public GameObject gameOverPanelPrefab;

    void Awake()
    {
        Debug.Log("GameInitializer: Inicializando juego...");

        // Paso 1: Asegurar que existe un GameManager
        GameManager gameManager = GameManager.GetInstance();
        if (gameManager == null)
        {
            Debug.LogError("No se pudo inicializar el GameManager. El juego no funcionará correctamente.");
            return;
        }

        Debug.Log("GameInitializer: GameManager inicializado correctamente.");

        // Paso 2: Configurar los sonidos si están disponibles
        ConfigureSounds(gameManager);

        // Paso 3: Configurar la UI si no existe ya
        ConfigureUI(gameManager);

        // Paso 4: Configurar el fondo personalizado
        ConfigureBackground();

        Debug.Log("GameInitializer: Inicialización del juego completada.");
    }    // Asigna los sonidos al GameManager
    private void ConfigureSounds(GameManager gameManager)
    {
        try
        {
            if (gameManager != null)
            {
                // Asignar sonidos solo si no están ya configurados
                if (gameManager.gameOverSound == null && gameOverSound != null)
                {
                    gameManager.gameOverSound = gameOverSound;
                    Debug.Log("GameInitializer: Sonido de Game Over asignado al GameManager.");
                }

                if (gameManager.scoreSound == null && scoreSound != null)
                {
                    gameManager.scoreSound = scoreSound;
                    Debug.Log("GameInitializer: Sonido de puntuación asignado al GameManager.");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("GameInitializer: Error al configurar los sonidos: " + e.Message);
        }
    }// Configura la UI del juego
    private void ConfigureUI(GameManager gameManager)
    {
        try
        {
            UISetup existingUISetup = FindAnyObjectByType<UISetup>();
            if (existingUISetup == null)
            {
                // Crear objeto para UISetup
                GameObject uiSetupObj = new GameObject("UIManager");
                UISetup uiSetup = uiSetupObj.AddComponent<UISetup>();

                // Asignar el GameManager al UISetup
                uiSetup.gameManager = gameManager;

                // Asignar los prefabs de UI si están disponibles
                if (scoreTextPrefab != null)
                    uiSetup.scoreTextPrefab = scoreTextPrefab;

                if (gameOverPanelPrefab != null)
                    uiSetup.gameOverPanelPrefab = gameOverPanelPrefab;

                Debug.Log("GameInitializer: UISetup creado y configurado automáticamente.");

                // Forzar la ejecución inmediata del setup
                uiSetup.SetupUI();
            }
            else
            {
                // Asegurar que el UISetup existente tenga asignado el GameManager
                if (existingUISetup.gameManager == null)
                {
                    existingUISetup.gameManager = gameManager;
                    Debug.Log("GameInitializer: GameManager asignado al UISetup existente.");
                }

                // Asignar los prefabs de UI si están disponibles y no asignados
                if (existingUISetup.scoreTextPrefab == null && scoreTextPrefab != null)
                    existingUISetup.scoreTextPrefab = scoreTextPrefab;

                if (existingUISetup.gameOverPanelPrefab == null && gameOverPanelPrefab != null)
                    existingUISetup.gameOverPanelPrefab = gameOverPanelPrefab;

                // Forzar la actualización de la UI
                existingUISetup.SetupUI();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("GameInitializer: Error al configurar la UI: " + e.Message);
        }
    }    // Configura el fondo personalizado
    private void ConfigureBackground()
    {
        Debug.Log("GameInitializer: Configurando fondo del juego...");

        // Paso 1: Configurar la cámara principal primero
        ConfigureMainCamera();

        // Paso 2: Eliminar TODOS los fondos no deseados
        RemoveUnwantedBackgrounds();

        // Verificar que tenemos un sprite válido
        if (customBackgroundSprite == null)
        {
            Debug.LogError("GameInitializer: No hay sprite de fondo asignado en el inspector. Asigna un sprite válido.");
            return;
        }

        // Verificar que el sprite es válido
        if (customBackgroundSprite.texture == null)
        {
            Debug.LogError("GameInitializer: El sprite de fondo no tiene textura. Verifica que sea un sprite válido.");
            return;
        }

        Debug.Log("GameInitializer: Sprite de fondo verificado: " +
            customBackgroundSprite.name + " (" +
            customBackgroundSprite.texture.width + "x" +
            customBackgroundSprite.texture.height + ")");

        try
        {
            // Buscar explícitamente por objeto CustomBackground y eliminarlo si existe
            GameObject existingCustomBg = GameObject.Find("CustomBackground");
            if (existingCustomBg != null)
            {
                Debug.Log("GameInitializer: Eliminando fondo existente antes de crear uno nuevo");
                Destroy(existingCustomBg);
            }

            // Verificar si existe un BackgroundSetup
            BackgroundSetup existingBackgroundSetup = FindAnyObjectByType<BackgroundSetup>();
            if (existingBackgroundSetup == null)
            {
                // Crear objeto para BackgroundSetup
                GameObject backgroundSetupObj = new GameObject("BackgroundManager");
                BackgroundSetup backgroundSetup = backgroundSetupObj.AddComponent<BackgroundSetup>();

                // Asignar el sprite personalizado
                backgroundSetup.customBackgroundSprite = customBackgroundSprite;
                Debug.Log("GameInitializer: Sprite de fondo personalizado asignado a nuevo BackgroundSetup: " + customBackgroundSprite.name);

                // Forzar la creación inmediata del fondo
                backgroundSetup.ForceBackgroundUpdate();
            }
            else
            {
                // Si existe un BackgroundSetup, asignar el sprite personalizado
                existingBackgroundSetup.customBackgroundSprite = customBackgroundSprite;
                Debug.Log("GameInitializer: Sprite de fondo personalizado asignado al BackgroundSetup existente: " + customBackgroundSprite.name);

                // Forzar la actualización del fondo
                existingBackgroundSetup.ForceBackgroundUpdate();
            }

            // Paso 3: Verificar que no queden fondos no deseados después de configurar el nuestro
            RemoveUnwantedBackgrounds();

            // Verificación adicional
            GameObject checkBg = GameObject.Find("CustomBackground");
            if (checkBg == null)
            {
                Debug.LogWarning("GameInitializer: Después de la configuración, no se encuentra el objeto CustomBackground");
            }
            else
            {
                SpriteRenderer sr = checkBg.GetComponent<SpriteRenderer>();
                if (sr == null || sr.sprite == null)
                {
                    Debug.LogWarning("GameInitializer: El objeto CustomBackground no tiene un SpriteRenderer válido");
                }
                else
                {
                    Debug.Log("GameInitializer: Fondo configurado correctamente con sprite: " + sr.sprite.name);
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("GameInitializer: Error al configurar el fondo: " + e.Message);
        }
    }// Configurar la cámara principal para asegurar que el fondo sea visible
    private void ConfigureMainCamera()
    {
        try
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {                // Configurar la cámara con un color negro transparente
                mainCamera.clearFlags = CameraClearFlags.SolidColor;
                mainCamera.backgroundColor = Color.clear; // Completamente transparente

                // Verificar que la cámara sea ortográfica (2D)
                if (!mainCamera.orthographic)
                {
                    Debug.LogWarning("GameInitializer: La cámara principal no está en modo ortográfico. Cambiando a modo ortográfico.");
                    mainCamera.orthographic = true;
                    mainCamera.orthographicSize = 5f; // Valor estándar
                }

                // Verificar la profundidad de la cámara
                Debug.Log("GameInitializer: Configuración de la cámara principal - " +
                    "Ortográfica: " + mainCamera.orthographic +
                    ", Tamaño ortográfico: " + mainCamera.orthographicSize +
                    ", Near: " + mainCamera.nearClipPlane +
                    ", Far: " + mainCamera.farClipPlane);

                // Ajustar los planos de recorte para asegurar la visibilidad del fondo
                if (mainCamera.nearClipPlane > 0.1f)
                {
                    mainCamera.nearClipPlane = 0.1f;
                    Debug.Log("GameInitializer: Se ajustó el plano near de la cámara a 0.1");
                }
                if (mainCamera.farClipPlane < 20f)
                {
                    mainCamera.farClipPlane = 20f;
                    Debug.Log("GameInitializer: Se ajustó el plano far de la cámara a 20 para asegurar la visibilidad del fondo");
                }

                // Verificar si hay componentes adicionales en la cámara que pudieran afectar la visualización
                var allComponents = mainCamera.GetComponents<Component>();
                foreach (var component in allComponents)
                {
                    if (component != null &&
                        !(component is Transform) &&
                        !(component is Camera) &&
                        !(component is AudioListener))
                    {
                        Debug.LogWarning("GameInitializer: La cámara tiene un componente adicional: " + component.GetType().Name);
                    }
                }

                Debug.Log("GameInitializer: Cámara principal configurada correctamente para mostrar solo nuestro fondo personalizado");
            }
            else
            {
                Debug.LogError("GameInitializer: No se encuentra la cámara principal (Main Camera)");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("GameInitializer: Error al configurar la cámara: " + e.Message);
        }
    }

    // Método para buscar y eliminar fondos verdes u otros fondos no deseados
    private void RemoveUnwantedBackgrounds()
    {
        Debug.Log("GameInitializer: Buscando y eliminando fondos no deseados...");
        try
        {
            // Buscar específicamente objetos SpawnerMarker y eliminarlos
            GameObject[] spawnerMarkers = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None)
                .Where(obj => obj.name == "SpawnerMarker").ToArray();

            foreach (GameObject marker in spawnerMarkers)
            {
                Debug.Log("GameInitializer: Encontrado y eliminando SpawnerMarker: " + marker.name);
                Destroy(marker);
            }

            // Buscar todos los GameObjects en la escena que podrían ser fondos
            GameObject[] allObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            int removedCount = 0;

            foreach (GameObject obj in allObjects)
            {
                bool shouldRemove = false;                // 1. Verificar por nombre si podría ser un fondo no deseado
                if ((obj.name.Contains("Background") ||
                     obj.name.Contains("backdrop") ||
                     obj.name.Contains("_bg") ||
                     obj.name.Contains("Plane")) &&
                    obj.name != "CustomBackground" &&
                    obj.name != "BackgroundManager" &&
                    obj.name != gameObject.name)
                {
                    Debug.Log("GameInitializer: Encontrado posible fondo por nombre: " + obj.name);
                    shouldRemove = true;
                }

                // 2. Verificar si tiene un Camera con color de fondo (que no sea negro transparente)
                Camera cam = obj.GetComponent<Camera>();
                if (cam != null && cam != Camera.main)
                {
                    Color bgColor = cam.backgroundColor;
                    if (bgColor.r > 0.1f || bgColor.g > 0.1f || bgColor.b > 0.1f || bgColor.a > 0)
                    {
                        Debug.Log("GameInitializer: Encontrada cámara con color de fondo: " + obj.name);
                        cam.backgroundColor = new Color(0, 0, 0, 0); // Hacer el fondo transparente
                        Debug.Log("GameInitializer: Ajustado color de fondo de cámara a transparente: " + obj.name);
                    }
                }

                // 3. Verificar si tiene un SpriteRenderer con color verde o algún color sólido de fondo
                SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
                if (sr != null)
                {                    // Si es verde (componente verde dominante)
                    Color color = sr.color;
                    if (color.g > color.r || color.g > color.b) // Menos restrictivo para detectar cualquier tinte verde
                    {
                        Debug.Log("GameInitializer: Encontrado fondo verde: " + obj.name + ", Color: " + color);
                        shouldRemove = true;
                    }
                    // Si no tiene sprite asignado pero es visible
                    else if (sr.sprite == null && sr.enabled)
                    {
                        Debug.Log("GameInitializer: Encontrado SpriteRenderer sin sprite asignado: " + obj.name);
                        shouldRemove = true;
                    }
                    // Si tiene nombre genérico
                    else if (obj.name.Contains("TestBackground") ||
                             obj.name.Contains("DefaultBackground") ||
                             obj.name.Contains("_background"))
                    {
                        Debug.Log("GameInitializer: Encontrado fondo con nombre genérico: " + obj.name);
                        shouldRemove = true;
                    }
                }

                // 4. Verificar si tiene un componente Renderer que no sea SpriteRenderer (como MeshRenderer)
                Renderer renderer = obj.GetComponent<Renderer>();
                if (renderer != null && !(renderer is SpriteRenderer) &&
                   (obj.name.Contains("Background") || obj.name.Contains("Plane")))
                {
                    Debug.Log("GameInitializer: Encontrado objeto con Renderer que podría ser un fondo: " + obj.name);
                    shouldRemove = true;
                }

                // Si se debe eliminar, hacerlo
                if (shouldRemove)
                {
                    Debug.Log("GameInitializer: Eliminando objeto de fondo no deseado: " + obj.name);
                    Destroy(obj);
                    removedCount++;
                }
            }

            Debug.Log("GameInitializer: Eliminados " + removedCount + " objetos de fondo no deseados");
        }
        catch (System.Exception e)
        {
            Debug.LogError("GameInitializer: Error al buscar fondos no deseados: " + e.Message);
        }
    }
}

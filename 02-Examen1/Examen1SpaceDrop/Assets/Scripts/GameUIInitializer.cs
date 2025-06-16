using UnityEngine;

public class GameUIInitializer : MonoBehaviour
{
    [Header("Configuración del Juego")]
    public string gameTitle = "Space Drop";

    [Header("Configuración Automática")]
    public bool createUIAutomatically = true;
    public bool createGameStateManager = true;
    public bool createMenuManager = true;

    void Awake()
    {
        Debug.Log("GameUIInitializer: Inicializando sistema de UI del juego...");

        // Asegurar que existe el GameManager
        GameManager gameManager = GameManager.GetInstance();
        if (gameManager == null)
        {
            Debug.LogError("GameUIInitializer: No se pudo crear el GameManager. El juego no funcionará correctamente.");
            return;
        }

        // Crear GameStateManager si no existe
        if (createGameStateManager && GameStateManager.Instance == null)
        {
            CreateGameStateManager();
        }

        // Crear MenuManager si no existe
        if (createMenuManager && MenuManager.Instance == null)
        {
            CreateMenuManager();
        }

        Debug.Log("GameUIInitializer: Inicialización completada.");
    }

    void Start()
    {
        // Verificar que todo esté configurado correctamente
        VerifySetup();
    }

    private void CreateGameStateManager()
    {
        GameObject gameStateObj = new GameObject("GameStateManager");
        gameStateObj.AddComponent<GameStateManager>();

        Debug.Log("GameUIInitializer: GameStateManager creado automáticamente.");
    }

    private void CreateMenuManager()
    {
        GameObject menuManagerObj = new GameObject("MenuManager");
        MenuManager menuManager = menuManagerObj.AddComponent<MenuManager>();

        // Configurar el título del juego
        menuManager.gameTitle = gameTitle;

        Debug.Log("GameUIInitializer: MenuManager creado automáticamente.");
    }

    private void VerifySetup()
    {
        bool allGood = true;

        // Verificar GameManager
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameUIInitializer: GameManager no encontrado.");
            allGood = false;
        }

        // Verificar GameStateManager
        if (GameStateManager.Instance == null)
        {
            Debug.LogError("GameUIInitializer: GameStateManager no encontrado.");
            allGood = false;
        }

        // Verificar MenuManager
        if (MenuManager.Instance == null)
        {
            Debug.LogError("GameUIInitializer: MenuManager no encontrado.");
            allGood = false;
        }

        // Verificar cámara principal
        if (Camera.main == null)
        {
            Debug.LogError("GameUIInitializer: No se encontró la cámara principal (Main Camera).");
            allGood = false;
        }

        if (allGood)
        {
            Debug.Log("GameUIInitializer: ✓ Todos los sistemas están configurados correctamente.");
        }
        else
        {
            Debug.LogWarning("GameUIInitializer: ⚠ Algunos sistemas no están configurados correctamente. Revisa los errores anteriores.");
        }
    }
}

using UnityEngine;

public enum GameState
{
    MainMenu,
    Playing,
    Paused,
    GameOver
}

public class GameStateManager : MonoBehaviour
{
    [Header("Estados del Juego")]
    public GameState currentState = GameState.MainMenu;

    // Singleton
    public static GameStateManager Instance { get; private set; }

    // Eventos para notificar cambios de estado
    public System.Action<GameState> OnStateChanged;

    void Awake()
    {
        // Configurar singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("GameStateManager: Instancia creada");
        }
        else
        {
            Debug.Log("GameStateManager: Instancia duplicada destruida");
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Empezar en el menú principal
        ChangeState(GameState.MainMenu);
    }

    public void ChangeState(GameState newState)
    {
        if (currentState == newState) return;

        GameState previousState = currentState;
        currentState = newState;

        Debug.Log($"GameStateManager: Cambiando estado de {previousState} a {newState}");

        // Manejar la lógica específica de cada estado
        HandleStateChange(previousState, newState);

        // Notificar a otros sistemas del cambio
        OnStateChanged?.Invoke(newState);
    }

    private void HandleStateChange(GameState previousState, GameState newState)
    {
        switch (newState)
        {
            case GameState.MainMenu:
                HandleMainMenuState();
                break;

            case GameState.Playing:
                HandlePlayingState();
                break;

            case GameState.Paused:
                HandlePausedState();
                break;

            case GameState.GameOver:
                HandleGameOverState();
                break;
        }
    }

    private void HandleMainMenuState()
    {
        // Pausar el tiempo del juego
        Time.timeScale = 0f;

        // El MenuManager se encargará de mostrar/ocultar los paneles
        Debug.Log("GameStateManager: Estado MainMenu activado");
    }

    private void HandlePlayingState()
    {
        // Reanudar el tiempo del juego
        Time.timeScale = 1f;

        Debug.Log("GameStateManager: Estado Playing activado");
    }

    private void HandlePausedState()
    {
        // Pausar el tiempo del juego
        Time.timeScale = 0f;

        Debug.Log("GameStateManager: Estado Paused activado");
    }

    private void HandleGameOverState()
    {
        // Pausar el tiempo del juego
        Time.timeScale = 0f;

        Debug.Log("GameStateManager: Estado GameOver activado");
    }

    // Métodos públicos para cambiar estados
    public void StartGame()
    {
        ChangeState(GameState.Playing);
    }

    public void PauseGame()
    {
        if (currentState == GameState.Playing)
        {
            ChangeState(GameState.Paused);
        }
    }

    public void ResumeGame()
    {
        if (currentState == GameState.Paused)
        {
            ChangeState(GameState.Playing);
        }
    }

    public void GameOver()
    {
        ChangeState(GameState.GameOver);
    }

    public void ReturnToMainMenu()
    {
        ChangeState(GameState.MainMenu);
    }

    public void QuitGame()
    {
        Debug.Log("GameStateManager: Saliendo del juego...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }    // Métodos de utilidad
    public bool IsInGame()
    {
        return currentState == GameState.Playing;
    }

    public bool IsPaused()
    {
        return currentState == GameState.Paused;
    }
}

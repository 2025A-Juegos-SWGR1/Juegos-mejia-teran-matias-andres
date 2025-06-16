using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Para trabajar con elementos de UI

public class GameManager : MonoBehaviour
{    // Singleton pattern para acceder al GameManager desde cualquier script
    public static GameManager Instance { get; private set; }    // Método estático para asegurar que exista una instancia del GameManager
    public static GameManager GetInstance()
    {
        if (Instance == null)
        {
            // Buscar si existe un GameManager en la escena
            GameManager existingManager = FindAnyObjectByType<GameManager>(); if (existingManager != null)
            {
                Instance = existingManager;
            }
            else
            {
                // Crear un nuevo GameManager si no existe
                GameObject managerObject = new GameObject("GameManager");
                Instance = managerObject.AddComponent<GameManager>();

                // Añadir un AudioSource para efectos de sonido
                if (managerObject.GetComponent<AudioSource>() == null)
                {
                    managerObject.AddComponent<AudioSource>();
                }                // Asegurarse de que no se destruya al cambiar de escena
                DontDestroyOnLoad(managerObject);
            }
        }

        return Instance;
    }    // Variables para controlar el estado del juego
    public bool isGameOver = false;
    public int score = 0;
    
    [Header("Sistema de Puntuación")]
    public int highScore = 0;     // Puntuación máxima alcanzada
    private const string HIGH_SCORE_KEY = "SpaceDrop_HighScore"; // Clave para guardar en PlayerPrefs

    [Header("Sistema de Vidas")]
    public int maxLives = 4;      // Número máximo de vidas
    public int currentLives = 4;  // Vidas actuales
    public float respawnDelay = 2f; // Tiempo antes de reaparecer

    [Header("UI Elements")]
    public Text scoreText;        // Texto para mostrar la puntuación
    public Text livesText;        // Texto para mostrar las vidas
    public Text highScoreText;    // Texto para mostrar la puntuación máxima
    public GameObject gameOverUI; // Panel de Game Over

    [Header("Audio")]
    public AudioClip gameOverSound; public AudioClip scoreSound;

    private AudioSource audioSource;

    // Variables para el sistema de respawn del jugador
    private GameObject playerPrefab;
    private Vector3 playerSpawnPosition = Vector3.zero;
    private bool isPlayerRespawning = false;

    void Awake()
    {
        try
        {
            // Singleton setup
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            // Inicializar componentes
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            // Cargar puntuación máxima guardada
            highScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
            Debug.Log("Puntuación máxima cargada: " + highScore);

            // Reiniciar el juego para asegurar que todas las variables estén en su estado inicial
            ResetGame();
        }
        catch (System.Exception e)
        {
            Debug.LogError("GameManager: Error en Awake: " + e.Message);
        }
    }// Variables para control de mensajes
    private bool gameOverWarningShown = false;
    private bool scoreWarningShown = false;
    private bool audioWarningShown = false; void Start()
    {
        // Inicializar el juego
        isGameOver = false;
        score = 0;
        currentLives = maxLives;

        // Inicializar UI después de un breve delay
        StartCoroutine(DelayedUIUpdate());
    }

    // Corrutina para dar tiempo a que la UI se configure
    System.Collections.IEnumerator DelayedUIUpdate()
    {
        // Esperar un pequeño tiempo para permitir que UISetup se ejecute primero
        yield return new WaitForSeconds(0.1f);        // Actualizar la UI después de la espera
        UpdateScoreUI();
        UpdateLivesUI();
        UpdateHighScoreUI();

        // Asegurarse de que el panel de Game Over esté oculto al inicio
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }
        else
        {
            // Solo mostrar este mensaje una vez
            if (!gameOverWarningShown)
            {
                Debug.LogWarning("GameManager: No hay un panel de GameOver asignado. La funcionalidad de Game Over será limitada.");
                gameOverWarningShown = true;
            }
        }
    }

    // Método para terminar el juego    // Método llamado cuando el jugador muere
    public void PlayerDied()
    {
        if (isGameOver || isPlayerRespawning)
        {
            Debug.Log($"PlayerDied() ignorado - isGameOver: {isGameOver}, isPlayerRespawning: {isPlayerRespawning}");
            return;
        }

        // Reducir vidas
        currentLives--;
        UpdateLivesUI();

        Debug.Log($"Jugador murió. Vidas restantes: {currentLives}/{maxLives}");

        if (currentLives <= 0)
        {
            // Sin vidas, game over
            Debug.Log("Sin vidas restantes, iniciando Game Over");
            GameOver();
        }
        else
        {
            // Aún hay vidas, reaparecer jugador
            Debug.Log($"Iniciando respawn, quedan {currentLives} vidas");
            Debug.Log($"Estado actual - isPlayerRespawning: {isPlayerRespawning}, playerPrefab null: {playerPrefab == null}");
            StartCoroutine(RespawnPlayer());
        }
    }    // Método para reaparecer al jugador
    private System.Collections.IEnumerator RespawnPlayer()
    {
        isPlayerRespawning = true;
        Debug.Log($"Iniciando proceso de respawn, esperando {respawnDelay} segundos...");

        // Esperar el tiempo de respawn
        yield return new WaitForSeconds(respawnDelay);

        // Buscar al jugador existente
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Debug.Log("Jugador encontrado pero debería haber sido destruido. Destruyendo...");
            Destroy(player);
        }

        // Crear nuevo jugador
        Debug.Log("Creando nuevo jugador...");
        CreateNewPlayer();

        isPlayerRespawning = false;
        Debug.Log("Proceso de respawn completado");
    }    // Método para crear un nuevo jugador
    private void CreateNewPlayer()
    {
        Debug.Log($"CreateNewPlayer() llamado. playerPrefab null? {playerPrefab == null}");

        if (playerPrefab != null)
        {
            Debug.Log($"Creando jugador en posición: {playerSpawnPosition}");

            // Crear nuevo jugador en la posición de spawn
            GameObject newPlayer = Instantiate(playerPrefab, playerSpawnPosition, Quaternion.identity);
            newPlayer.name = "Player";
            newPlayer.SetActive(true); // Asegurar que esté activo

            Debug.Log($"Nuevo jugador creado: {newPlayer.name}");
        }
        else
        {
            Debug.LogError("ERROR: No hay prefab de jugador asignado para crear uno nuevo. Verificar que PlayerController esté configurando el prefab correctamente.");
        }
    }    public void GameOver()
    {
        if (!isGameOver)
        {
            isGameOver = true;

            Debug.Log("Game Over!");

            if (audioSource != null && gameOverSound != null)
            {
                audioSource.PlayOneShot(gameOverSound);
            }

            // Notificar al GameStateManager
            GameStateManager gameStateManager = GameStateManager.Instance;
            if (gameStateManager != null)
            {
                gameStateManager.GameOver();
            }
            else
            {
                // Comportamiento de respaldo si no hay GameStateManager
                // Mostrar panel de Game Over
                if (gameOverUI != null)
                {
                    gameOverUI.SetActive(true);
                }
                else
                {
                    // Solo mostrar el warning una vez
                    if (!gameOverWarningShown)
                    {
                        Debug.LogWarning("No hay panel de GameOver asignado - la funcionalidad será limitada");
                        gameOverWarningShown = true;
                    }
                }

                // Reiniciar el juego después de un tiempo (comportamiento original)
                Invoke("RestartGame", 3f);
            }
        }
    }// Método para reiniciar el juego
    public void RestartGame()
    {
        // Reinicia las variables del juego        ResetGame();

        // Recarga la escena actual
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    // Método para aumentar la puntuación
    public void AddScore(int points)
    {
        if (!isGameOver)
        {
            score += points;
            
            // Verificar si se superó la puntuación máxima
            if (score > highScore)
            {
                highScore = score;
                SaveHighScore();
                Debug.Log("¡Nuevo récord! Puntuación máxima: " + highScore);
            }

            // Actualizar UI
            UpdateScoreUI();
            UpdateHighScoreUI();

            // Reproducir sonido de puntuación
            if (audioSource != null && scoreSound != null)
            {
                audioSource.PlayOneShot(scoreSound);
            }
        }
    }

    // Método para actualizar la UI de puntuación
    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Puntuación: " + score;
        }
        else
        {
            // Solo mostrar el mensaje la primera vez para no llenar la consola
            if (!scoreWarningShown)
            {
                Debug.LogWarning("GameManager: No hay texto de puntuación asignado en el GameManager - la puntuación solo se mostrará en la consola");
                scoreWarningShown = true;
            }

            // Al menos mostramos la puntuación en la consola
            if (score > 0)
            {
                Debug.Log("GameManager: Puntuación actual: " + score);
            }
        }
        
        // Notificar al MenuManager si existe
        MenuManager menuManager = MenuManager.Instance;
        if (menuManager != null)
        {
            menuManager.UpdateScore(score);
        }
    }

    // Método para actualizar la UI de puntuación máxima
    private void UpdateHighScoreUI()
    {
        if (highScoreText != null)
        {
            highScoreText.text = "Récord: " + highScore;
        }
        
        // Notificar al MenuManager si existe
        MenuManager menuManager = MenuManager.Instance;
        if (menuManager != null)
        {
            menuManager.UpdateHighScore(highScore);
        }
    }

    // Método para guardar la puntuación máxima
    private void SaveHighScore()
    {
        PlayerPrefs.SetInt(HIGH_SCORE_KEY, highScore);
        PlayerPrefs.Save();
        Debug.Log("Puntuación máxima guardada: " + highScore);
    }

    // Método público para obtener la puntuación máxima
    public int GetHighScore()
    {
        return highScore;
    }
    // Método público para obtener la puntuación actual
    public int GetScore()
    {
        return score;
    }

    // Método público para reiniciar las estadísticas del juego (llamado desde el menú)
    public void ResetStats()
    {
        PlayerPrefs.DeleteKey(HIGH_SCORE_KEY);
        PlayerPrefs.Save();
        highScore = 0;
        UpdateHighScoreUI();
        Debug.Log("Estadísticas del juego reiniciadas");
    }    // Método para reiniciar las variables del juego
    public void ResetGame()
    {
        try
        {
            // Reiniciar variables
            score = 0;
            currentLives = maxLives;
            isGameOver = false;
            isPlayerRespawning = false;            // Actualizar UI
            UpdateScoreUI();
            UpdateLivesUI();
            UpdateHighScoreUI();

            // Ocultar panel de Game Over si existe
            if (gameOverUI != null)
            {
                gameOverUI.SetActive(false);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("GameManager: Error al reiniciar el juego: " + e.Message);
        }
    }    // Método para actualizar la UI de vidas
    private void UpdateLivesUI()
    {
        if (livesText != null)
        {
            livesText.text = "Vidas: " + currentLives;
        }
        
        // Notificar al MenuManager si existe
        MenuManager menuManager = MenuManager.Instance;
        if (menuManager != null)
        {
            menuManager.UpdateLives(currentLives);
        }
    }

    // Métodos públicos para el sistema de vidas
    public int GetCurrentLives()
    {
        return currentLives;
    }

    public int GetMaxLives()
    {
        return maxLives;
    }

    public bool IsPlayerRespawning()
    {
        return isPlayerRespawning;
    }    // Método para configurar el prefab del jugador (llamado desde el jugador en Start)
    public void SetPlayerPrefab(GameObject prefab, Vector3 spawnPos)
    {
        playerPrefab = prefab;
        playerSpawnPosition = spawnPos;
        Debug.Log($"SetPlayerPrefab() llamado. Prefab: {prefab?.name}, Posición: {spawnPos}");
        Debug.Log($"playerPrefab configurado correctamente: {playerPrefab != null}");
    }

    // Método para agregar vidas (power-up futuro)
    public void AddLife()
    {
        if (currentLives < maxLives)
        {
            currentLives++;
            UpdateLivesUI();
            Debug.Log($"Vida extra! Vidas actuales: {currentLives}");
        }
    }
}

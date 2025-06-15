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
            GameManager existingManager = FindAnyObjectByType<GameManager>();

            if (existingManager != null)
            {
                Instance = existingManager;
                Debug.Log("GameManager: GameManager encontrado en la escena.");
            }
            else
            {
                // Crear un nuevo GameManager si no existe
                GameObject managerObject = new GameObject("GameManager");
                Instance = managerObject.AddComponent<GameManager>();

                // Añadir un AudioSource si es necesario
                if (managerObject.GetComponent<AudioSource>() == null)
                {
                    managerObject.AddComponent<AudioSource>();
                }

                // Intentar crear elementos básicos de UI si es posible
                CreateBasicUI();

                // Asegurarse de que no se destruya al cambiar de escena
                DontDestroyOnLoad(managerObject);

                Debug.Log("GameManager: GameManager creado automáticamente porque no existía en la escena.");
            }
        }

        return Instance;
    }

    // Método para crear elementos básicos de UI
    private static void CreateBasicUI()
    {
        try
        {            // Buscar si ya existe un Canvas en la escena
            Canvas existingCanvas = FindAnyObjectByType<Canvas>();

            if (existingCanvas != null)
            {
                Debug.Log("Se encontró un Canvas existente. No se creará UI automáticamente.");
                return;
            }

            // En juegos simples puede ser útil crear un canvas básico,
            // pero esto solo debe hacerse en entornos de desarrollo o prueba
            Debug.Log("Creación de UI básica omitida - añade elementos de UI manualmente para mejor control");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("No se pudo crear UI básica: " + e.Message);
        }
    }

    // Variables para controlar el estado del juego
    public bool isGameOver = false;
    public int score = 0;

    [Header("UI Elements")]
    public Text scoreText;        // Texto para mostrar la puntuación
    public GameObject gameOverUI; // Panel de Game Over

    [Header("Audio")]
    public AudioClip gameOverSound;
    public AudioClip scoreSound; private AudioSource audioSource;

    void Awake()
    {
        Debug.Log("GameManager: Awake iniciado");

        try
        {
            // Singleton setup
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Debug.Log("GameManager: Inicializado en Awake como singleton.");
            }
            else if (Instance != this)
            {
                Debug.Log("GameManager: Se destruyó una instancia duplicada de GameManager.");
                Destroy(gameObject);
                return;
            }

            // Inicializar componentes
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                Debug.Log("GameManager: AudioSource añadido al GameManager.");
            }

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
    private bool audioWarningShown = false;

    void Start()
    {
        // Inicializar el juego
        isGameOver = false;
        score = 0;

        // Si no hay UI configurada, dar tiempo para que UISetup se ejecute
        StartCoroutine(DelayedUIUpdate());

        Debug.Log("GameManager: juego inicializado. Puntuación: " + score);
    }

    // Corrutina para dar tiempo a que la UI se configure
    System.Collections.IEnumerator DelayedUIUpdate()
    {
        // Esperar un pequeño tiempo para permitir que UISetup se ejecute primero
        yield return new WaitForSeconds(0.1f);

        // Actualizar la UI después de la espera
        UpdateScoreUI();

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

    // Método para terminar el juego
    public void GameOver()
    {
        if (!isGameOver)
        {
            isGameOver = true;
            Debug.Log("¡Juego terminado! Puntuación final: " + score);            // Reproducir sonido de Game Over
            if (audioSource != null && gameOverSound != null)
            {
                audioSource.PlayOneShot(gameOverSound);
            }
            else
            {
                // Solo mostrar el warning una vez
                if (!audioWarningShown)
                {
                    Debug.LogWarning("No se pudo reproducir sonido de GameOver (audioSource o clip no disponible)");
                    audioWarningShown = true;
                }
            }

            // Mostrar panel de Game Over
            if (gameOverUI != null)
            {
                gameOverUI.SetActive(true);
                Debug.Log("Panel de Game Over activado");
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

            // Reiniciar el juego después de un tiempo
            Invoke("RestartGame", 3f);
            Debug.Log("Juego se reiniciará en 3 segundos");
        }
    }

    // Método para reiniciar el juego
    public void RestartGame()
    {
        // Reinicia las variables del juego
        isGameOver = false;
        score = 0;

        // Recarga la escena actual
        Scene currentScene = SceneManager.GetActiveScene(); SceneManager.LoadScene(currentScene.name);
    }    // Método para aumentar la puntuación
    public void AddScore(int points)
    {
        if (!isGameOver)
        {
            score += points;
            Debug.Log("GameManager: Puntuación: " + score);

            // Actualizar UI
            UpdateScoreUI();

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
    }

    // Método para reiniciar el juego
    public void ResetGame()
    {
        Debug.Log("GameManager: Reiniciando juego...");

        try
        {
            // Reiniciar variables
            score = 0;
            isGameOver = false;

            // Actualizar UI
            UpdateScoreUI();

            // Ocultar panel de Game Over si existe
            if (gameOverUI != null)
            {
                gameOverUI.SetActive(false);
            }

            Debug.Log("GameManager: Juego reiniciado correctamente.");
        }
        catch (System.Exception e)
        {
            Debug.LogError("GameManager: Error al reiniciar el juego: " + e.Message);
        }
    }
}

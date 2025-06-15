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
            GameManager existingManager = FindAnyObjectByType<GameManager>();            if (existingManager != null)
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
    }

    // Variables para controlar el estado del juego
    public bool isGameOver = false;
    public int score = 0;

    [Header("UI Elements")]
    public Text scoreText;        // Texto para mostrar la puntuación
    public GameObject gameOverUI; // Panel de Game Over

    [Header("Audio")]
    public AudioClip gameOverSound;    public AudioClip scoreSound;
    
    private AudioSource audioSource;

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
    {        // Inicializar el juego
        isGameOver = false;
        score = 0;
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
    {        if (!isGameOver)
        {
            isGameOver = true;            if (audioSource != null && gameOverSound != null)
            {
                audioSource.PlayOneShot(gameOverSound);
            }            // Mostrar panel de Game Over
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
            }            // Reiniciar el juego después de un tiempo
            Invoke("RestartGame", 3f);
        }
    }    // Método para reiniciar el juego
    public void RestartGame()
    {
        // Reinicia las variables del juego
        ResetGame();

        // Recarga la escena actual
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }// Método para aumentar la puntuación
    public void AddScore(int points)
    {        if (!isGameOver)
        {
            score += points;

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
    }    // Método para reiniciar las variables del juego
    public void ResetGame()
    {
        try
        {
            // Reiniciar variables
            score = 0;
            isGameOver = false;

            // Actualizar UI
            UpdateScoreUI();

            // Ocultar panel de Game Over si existe            if (gameOverUI != null)
            {
                gameOverUI.SetActive(false);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("GameManager: Error al reiniciar el juego: " + e.Message);
        }
    }
}

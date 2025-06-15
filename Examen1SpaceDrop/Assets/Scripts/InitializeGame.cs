using UnityEngine;

// Este script debe añadirse a un objeto en la escena inicial para garantizar la inicialización del juego
public class InitializeGame : MonoBehaviour
{
    void Awake()
    {
        Debug.Log("InitializeGame: Inicializando el juego...");

        // Inicializar GameManager
        InitializeGameManager();
    }

    void Start()
    {
        // Verificar componentes críticos
        VerifyGameComponents();
    }

    // Método para inicializar el GameManager
    private void InitializeGameManager()
    {
        try
        {
            GameManager gameManager = GameManager.GetInstance();
            if (gameManager != null)
            {
                Debug.Log("InitializeGame: GameManager inicializado correctamente");
                gameManager.ResetGame();
            }
            else
            {
                Debug.LogError("InitializeGame: No se pudo obtener una instancia de GameManager");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("InitializeGame: Error al inicializar el GameManager: " + e.Message);
        }
    }

    // Método para verificar componentes críticos del juego
    private void VerifyGameComponents()
    {
        // Verificar cámara principal
        if (Camera.main == null)
        {
            Debug.LogError("InitializeGame: No se encontró la cámara principal (Main Camera)");
        }

        // Verificar jugador
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("InitializeGame: No se encontró un objeto con tag 'Player' en la escena");
        }
        else
        {
            Debug.Log("InitializeGame: Jugador encontrado en posición " + player.transform.position);
        }
    }
}

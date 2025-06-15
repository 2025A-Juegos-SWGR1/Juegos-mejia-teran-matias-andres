using UnityEngine;

// Este script debe añadirse a un objeto persistente en la escena inicial
public class InitializeGame : MonoBehaviour
{
    void Awake()
    {
        Debug.Log("InitializeGame: Awake iniciado - Intentando inicializar el juego...");

        // Intentar inicializar el GameManager
        try
        {
            GameManager gameManager = GameManager.GetInstance();
            if (gameManager != null)
            {
                Debug.Log("InitializeGame: GameManager inicializado correctamente");

                // Reiniciar el juego para asegurar el estado inicial correcto
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

    void Start()
    {
        Debug.Log("InitializeGame: Start iniciado - Verificando componentes del juego...");

        // Verificar que exista la cámara principal
        if (Camera.main == null)
        {
            Debug.LogError("InitializeGame: No se encontró la cámara principal (Main Camera)");
        }

        // Verificar que exista un objeto jugador
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("InitializeGame: No se encontró un objeto con tag 'Player' en la escena");
        }
        else
        {
            Debug.Log("InitializeGame: Se encontró al jugador en la posición " + player.transform.position);
        }
    }
}

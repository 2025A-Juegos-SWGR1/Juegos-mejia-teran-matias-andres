using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody2D rb;
    private Vector2 movement;
    public float limitX = 8.5f; // Límite horizontal de la pantalla
    public float limitY = 4.5f; // Límite vertical de la pantalla

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Verificar que el componente Rigidbody2D existe
        if (rb == null)
        {
            Debug.LogError("No se encontró Rigidbody2D en el objeto del jugador. Se añadirá uno automáticamente.");
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f; // Sin gravedad para control manual
        }

        // Verificar que el tag del jugador es correcto
        if (!CompareTag("Player"))
        {
            Debug.LogWarning("El objeto del jugador no tiene el tag 'Player'. Esto puede causar problemas con las colisiones.");
        }

        // Asegurarse de que existe un GameManager en la escena
        try
        {
            GameManager gameManager = GameManager.GetInstance();
            Debug.Log("GameManager inicializado correctamente al inicio del juego.");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error al inicializar el GameManager: " + e.Message);
        }
    }

    void Update()
    {
        // Capturar input de movimiento
        float moveHorizontal = Input.GetAxis("Horizontal"); // A y D, o flechas izquierda y derecha
        float moveVertical = Input.GetAxis("Vertical");     // W y S, o flechas arriba y abajo

        // Crear vector de movimiento
        movement = new Vector2(moveHorizontal, moveVertical);
    }

    void FixedUpdate()
    {
        // Aplicar movimiento en FixedUpdate para mejor comportamiento físico
        MovePlayer();
        ClampPosition();
    }
    void MovePlayer()
    {        // Mover la nave con velocidad normalizada para movimiento uniforme en diagonal
        if (movement.magnitude > 0)
        {
            // Normalizar para evitar movimiento más rápido en diagonal
            Vector2 direction = movement.normalized;
            rb.linearVelocity = direction * speed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    void ClampPosition()
    {
        // Restringir la posición de la nave dentro de los límites de la pantalla
        Vector2 position = rb.position;
        position.x = Mathf.Clamp(position.x, -limitX, limitX);
        position.y = Mathf.Clamp(position.y, -limitY, limitY);
        rb.position = position;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Asteroid"))
        {
            try
            {
                // Usar GetInstance para asegurar que existe un GameManager
                GameManager gameManager = GameManager.GetInstance();
                gameManager.GameOver();
                Debug.Log("GameOver llamado correctamente después de colisión con asteroide.");

                // Destruir la nave del jugador
                Destroy(gameObject);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error al acceder al GameManager: " + e.Message);
                // Alternativa temporal si no hay GameManager
                Debug.Log("¡Colisión con asteroide! Game Over");

                // Destruir la nave del jugador incluso si hay un error
                Destroy(gameObject);
            }
        }
    }
}
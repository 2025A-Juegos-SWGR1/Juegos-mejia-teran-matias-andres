using UnityEngine;

public class AsteroidController : MonoBehaviour
{
    public float speed = 3f;           // Velocidad del asteroide
    public float rotationSpeed = 50f;  // Velocidad de rotación
    public int pointValue = 10;        // Puntos al destruir este asteroide
    public GameObject explosionPrefab; // Prefab de explosión (opcional)

    // Para depuración
    public bool showDebugInfo = true;

    private Vector2 direction = Vector2.down;  // Dirección de movimiento (por defecto hacia abajo)
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool isInitialized = false;

    void Awake()
    {
        // Obtener componentes necesarios
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Verificar Rigidbody2D
        if (rb == null)
        {
            Debug.LogError("No se encontró Rigidbody2D en el asteroide. Añadiendo automáticamente.");
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
        }

        // Verificar SpriteRenderer
        if (spriteRenderer == null)
        {
            Debug.LogError("No se encontró SpriteRenderer en el asteroide. Asegúrate de que el prefab tenga este componente.");
        }
        else
        {
            // Confirmar que el sprite es visible
            if (spriteRenderer.sprite == null)
            {
                Debug.LogError("El SpriteRenderer no tiene un sprite asignado. Asigna un sprite al asteroide.");
            }
        }
    }
    void Start()
    {
        // Asignar una rotación aleatoria
        float randomRotation = Random.Range(0f, 360f);
        transform.rotation = Quaternion.Euler(0f, 0f, randomRotation);        // Si ya se ha establecido una dirección mediante SetDirection, usarla
        // De lo contrario, usar la dirección por defecto (hacia abajo)
        rb.linearVelocity = direction * speed;
        isInitialized = true;

        if (showDebugInfo)
        {
            Debug.Log($"Asteroide inicializado en posición {transform.position} con dirección {direction} y velocidad {speed}");
        }
    }

    void Update()
    {
        // Rotar el asteroide continuamente
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);        // Verificar que el asteroide sea visible
        if (showDebugInfo && Time.frameCount % 60 == 0) // Solo cada 60 frames para no spamear la consola
        {
            Debug.Log($"Asteroide en posición {transform.position} con velocidad {rb.linearVelocity}");
        }

        // Destruir el asteroide si sale de la pantalla
        if (transform.position.y < -6f ||
            transform.position.y > 6f ||
            transform.position.x < -10f ||
            transform.position.x > 10f)
        {
            if (showDebugInfo)
            {
                Debug.Log($"Asteroide destruido por salir de la pantalla: {transform.position}");
            }
            Destroy(gameObject);
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        // Si choca con un disparo del jugador
        if (IsPlayerBullet(other.gameObject))
        {
            HandleBulletCollision(other.gameObject);
            return; // Ya procesamos la colisión, salir
        }

        // Si choca con el jugador
        if (IsPlayer(other.gameObject))
        {
            HandlePlayerCollision(other.gameObject);
        }
    }

    // Método para verificar si es un disparo del jugador
    private bool IsPlayerBullet(GameObject obj)
    {
        return obj.CompareTag("PlayerBullet") ||
               obj.name.Contains("Bullet") ||
               obj.name.Contains("bullet");
    }

    // Método para verificar si es el jugador
    private bool IsPlayer(GameObject obj)
    {
        return obj.CompareTag("Player") ||
               obj.name.Contains("Player") ||
               obj.name.Contains("player");
    }

    // Método para manejar la colisión con un disparo
    private void HandleBulletCollision(GameObject bullet)
    {
        // Sumar puntos
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(pointValue);
        }

        // Instanciar explosión si existe
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        // Destruir el disparo
        Destroy(bullet);

        // Destruir el asteroide
        Destroy(gameObject);

        if (showDebugInfo)
        {
            Debug.Log("Asteroide destruido por disparo del jugador");
        }
    }

    // Método para manejar la colisión con el jugador
    private void HandlePlayerCollision(GameObject player)
    {
        if (showDebugInfo)
        {
            Debug.Log("Asteroide colisionó con el jugador");
        }

        // Instanciar explosión para el jugador si existe el prefab
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, player.transform.position, Quaternion.identity);
        }

        // Destruir la nave del jugador
        Destroy(player);

        // Notificar al GameManager si existe
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
    }// Método para establecer una dirección personalizada
    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection.normalized;        // Si ya está inicializado, actualizar la velocidad inmediatamente
        if (isInitialized && rb != null)
        {
            rb.linearVelocity = direction * speed;
            if (showDebugInfo)
            {
                Debug.Log($"Dirección del asteroide actualizada a {direction} con velocidad {rb.linearVelocity}");
            }
        }
        // Si no, la dirección se aplicará en el método Start
    }
}

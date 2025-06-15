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
        // Si choca con un disparo del jugador - comprobar si existe la etiqueta primero
        try
        {
            // Solo intentar comparar con PlayerBullet si la etiqueta existe
            if (other.gameObject.tag == "PlayerBullet" || other.name.Contains("Bullet") || other.name.Contains("bullet"))
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
                Destroy(other.gameObject);

                // Destruir el asteroide
                Destroy(gameObject);

                return; // Ya procesamos la colisión, salir
            }
        }
        catch (System.Exception)
        {
            // Ignorar errores de etiquetas no definidas
            if (showDebugInfo)
            {
                Debug.Log("Advertencia: La etiqueta PlayerBullet no está definida. Considera crearla en Edit > Project Settings > Tags and Layers");
            }
        }        // Si choca con el jugador - verificar si existe la etiqueta
        try
        {
            if (other.CompareTag("Player") || other.name.Contains("Player") || other.name.Contains("player"))
            {
                if (showDebugInfo)
                {
                    Debug.Log("Asteroide colisionó con el jugador");
                }

                // Instanciar explosión para el jugador si existe el prefab
                if (explosionPrefab != null)
                {
                    Instantiate(explosionPrefab, other.transform.position, Quaternion.identity);
                }

                // Destruir la nave del jugador
                Destroy(other.gameObject);

                // Notificar al GameManager si existe
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.GameOver();
                }
            }
        }
        catch (System.Exception)
        {
            // Ignorar errores de etiquetas
            if (showDebugInfo)
            {
                Debug.Log("Advertencia: La etiqueta Player no está definida. Considera crearla en Edit > Project Settings > Tags and Layers");
            }
        }
    }    // Método para establecer una dirección personalizada
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

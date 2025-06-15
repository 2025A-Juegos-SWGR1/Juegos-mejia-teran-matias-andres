using UnityEngine;

public class CrystalController : MonoBehaviour
{
    public float speed = 2f;           // Velocidad del cristal
    public float rotationSpeed = 30f;  // Velocidad de rotación
    public int pointValue = 50;        // Puntos al destruir este cristal
    public GameObject explosionPrefab; // Prefab de explosión (opcional)

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
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
        }

        // Verificar SpriteRenderer
        if (spriteRenderer == null)
        {
            Debug.LogError("No se encontró SpriteRenderer en el cristal. Asegúrate de que el prefab tenga este componente.");
        }
        else
        {
            // Confirmar que el sprite es visible
            if (spriteRenderer.sprite == null)
            {
                Debug.LogError("El SpriteRenderer no tiene un sprite asignado. Asigna un sprite al cristal.");
            }
        }
    }

    void Start()
    {
        // Asignar una rotación aleatoria
        float randomRotation = Random.Range(0f, 360f);
        transform.rotation = Quaternion.Euler(0f, 0f, randomRotation);

        // Si ya se ha establecido una dirección mediante SetDirection, usarla
        // De lo contrario, usar la dirección por defecto (hacia abajo)
        rb.linearVelocity = direction * speed;
        isInitialized = true;
    }

    void Update()
    {
        // Rotar el cristal continuamente
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

        // Destruir el cristal si sale de la pantalla
        if (transform.position.y < -6f ||
            transform.position.y > 6f ||
            transform.position.x < -10f ||
            transform.position.x > 10f)
        {
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

        // Destruir el cristal
        Destroy(gameObject);
    }
    
    // Método para manejar la colisión con el jugador (opcional: poder-up)
    private void HandlePlayerCollision(GameObject player)
    {
        // Los cristales podrían dar puntos al tocarlos en lugar de dañar
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(pointValue / 2); // Menos puntos que al disparar
        }

        // Instanciar explosión si existe el prefab
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        // Destruir el cristal (no destruir al jugador)
        Destroy(gameObject);
    }

    // Método para establecer una dirección personalizada
    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection.normalized;

        // Si ya está inicializado, actualizar la velocidad inmediatamente
        if (isInitialized && rb != null)
        {
            rb.linearVelocity = direction * speed;
        }
        // Si no, la dirección se aplicará en el método Start
    }
}

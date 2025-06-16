using UnityEngine;

public enum AsteroidSize
{
    Small,   // 1 impacto
    Medium,  // 2 impactos
    Large    // 3 impactos
}

public class AsteroidController : MonoBehaviour
{
    [Header("Configuración básica del asteroide")]
    public float speed = 3f;           // Velocidad del asteroide
    public float rotationSpeed = 50f;  // Velocidad de rotación
    public GameObject explosionPrefab; // Prefab de explosión (opcional)

    [Header("Configuración del tamaño del asteroide")]
    public AsteroidSize asteroidSize = AsteroidSize.Medium;  // Tamaño del asteroide
    public int pointValue = 10;        // Puntos al destruir este asteroide (valor por defecto)

    [Header("Configuración de sprites por tamaño")]
    public Sprite smallSprite;         // Sprite para asteroide pequeño
    public Sprite mediumSprite;        // Sprite para asteroide mediano
    public Sprite largeSprite;         // Sprite para asteroide grande

    [Header("Configuración de escalas por tamaño")]
    public float smallScale = 0.5f;    // Escala del asteroide pequeño
    public float mediumScale = 1.0f;   // Escala del asteroide mediano
    public float largeScale = 1.5f;    // Escala del asteroide grande

    [Header("Depuración")]
    public bool showDebugInfo = false; private Vector2 direction = Vector2.down;  // Dirección de movimiento (por defecto hacia abajo)
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool isInitialized = false;

    // Variables para el sistema de salud/impactos
    private int maxHealth;      // Salud máxima según el tamaño
    private int currentHealth;  // Salud actual
    private Color originalColor; // Color original del sprite

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
        // Configurar el asteroide basado en su tamaño
        ConfigureAsteroidBySize();

        // Guardar el color original
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

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
        // Rotar el asteroide continuamente
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

        // Destruir el asteroide si sale de la pantalla
        if (transform.position.y < -6f ||
            transform.position.y > 6f || transform.position.x < -10f ||
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
    }    // Método para manejar la colisión con un disparo
    private void HandleBulletCollision(GameObject bullet)
    {
        // Destruir el disparo inmediatamente
        Destroy(bullet);

        // Reducir la salud del asteroide
        currentHealth--;

        if (showDebugInfo)
        {
            Debug.Log($"Asteroide {asteroidSize} impactado. Salud: {currentHealth}/{maxHealth}");
        }

        // Efecto visual de daño
        ShowDamageEffect();

        // Si el asteroide aún tiene salud, continuar
        if (currentHealth > 0)
        {
            return;
        }

        // El asteroide fue destruido - dar puntos
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(pointValue);
        }

        // Instanciar explosión si existe
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        // Destruir el asteroide
        Destroy(gameObject);
    }// Método para manejar la colisión con el jugador
    private void HandlePlayerCollision(GameObject player)
    {
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
    }    // Método para establecer una dirección personalizada
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

    // Método para establecer el tamaño del asteroide
    public void SetAsteroidSize(AsteroidSize size)
    {
        asteroidSize = size;
        ConfigureAsteroidBySize();
    }

    // Método para configurar el asteroide basado en su tamaño
    private void ConfigureAsteroidBySize()
    {
        switch (asteroidSize)
        {
            case AsteroidSize.Small:
                maxHealth = 1;
                currentHealth = maxHealth;
                pointValue = 30;    // Más puntos por ser más fácil de destruir
                ApplyAsteroidSettings(smallSprite, smallScale);
                break;
            case AsteroidSize.Medium:
                maxHealth = 2;
                currentHealth = maxHealth;
                pointValue = 20;    // Puntos moderados
                ApplyAsteroidSettings(mediumSprite, mediumScale);
                break;
            case AsteroidSize.Large:
                maxHealth = 3;
                currentHealth = maxHealth;
                pointValue = 10;    // Menos puntos pero más resistente
                ApplyAsteroidSettings(largeSprite, largeScale);
                break;
        }
    }

    // Método para aplicar sprite y escala al asteroide
    private void ApplyAsteroidSettings(Sprite sprite, float scale)
    {
        if (spriteRenderer != null && sprite != null)
        {
            spriteRenderer.sprite = sprite;
        }

        // Aplicar escala
        transform.localScale = Vector3.one * scale;
    }

    // Método para mostrar efecto visual de daño
    private void ShowDamageEffect()
    {
        if (spriteRenderer != null)
        {
            // Cambiar temporalmente a color rojo para indicar daño
            StartCoroutine(DamageFlash());
        }
    }

    // Corrutina para el efecto de parpadeo cuando recibe daño
    private System.Collections.IEnumerator DamageFlash()
    {
        if (spriteRenderer != null)
        {
            // Cambiar a rojo
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);

            // Volver al color original
            spriteRenderer.color = originalColor;
        }
    }

    // Métodos públicos para obtener información del asteroide
    public AsteroidSize GetAsteroidSize()
    {
        return asteroidSize;
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public int GetPointValue()
    {
        return pointValue;
    }
}

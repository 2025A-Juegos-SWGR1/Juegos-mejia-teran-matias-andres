using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 5f;
    public float limitX = 8.5f; // Límite horizontal de la pantalla
    public float limitY = 4.5f; // Límite vertical de la pantalla

    [Header("Disparos")]
    public GameObject bulletPrefab;         // Prefab de la bala
    public Transform firePoint;            // Punto desde donde salen las balas (opcional)
    public float fireRate = 0.3f;          // Tiempo entre disparos
    public AudioClip shootSound;           // Sonido del disparo (opcional)

    private Rigidbody2D rb;
    private Vector2 movement;
    private float nextFireTime = 0f;
    private AudioSource audioSource; void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        // Verificar que el componente Rigidbody2D existe
        if (rb == null)
        {
            Debug.LogError("No se encontró Rigidbody2D en el objeto del jugador. Se añadirá uno automáticamente.");
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f; // Sin gravedad para control manual
        }

        // Añadir AudioSource si no existe
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Verificar que el tag del jugador es correcto
        if (!CompareTag("Player"))
        {
            Debug.LogWarning("El objeto del jugador no tiene el tag 'Player'. Esto puede causar problemas con las colisiones.");
        }        // Asegurarse de que existe un GameManager en la escena
        try
        {
            GameManager gameManager = GameManager.GetInstance();
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

        // Capturar input de disparo
        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
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
            HandleAsteroidCollision();
        }
    }

    private void HandleAsteroidCollision()
    {
        try
        {            // Obtener el GameManager
            GameManager gameManager = GameManager.GetInstance();
            gameManager.GameOver();

            // Destruir la nave del jugador
            Destroy(gameObject);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error al procesar colisión con asteroide: " + e.Message);

            // Asegurar que el jugador sea destruido incluso si hay error
            Destroy(gameObject);
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null)
        {
            Debug.LogWarning("No hay prefab de bala asignado al PlayerController.");
            return;
        }

        // Determinar la posición de disparo
        Vector3 shootPosition;
        if (firePoint != null)
        {
            shootPosition = firePoint.position;
        }
        else
        {
            // Si no hay firePoint, disparar desde la parte superior del jugador
            shootPosition = transform.position + Vector3.up * 0.5f;
        }

        // Crear la bala
        GameObject bullet = Instantiate(bulletPrefab, shootPosition, Quaternion.identity);

        // Asignar el tag PlayerBullet si no lo tiene
        if (!bullet.CompareTag("PlayerBullet"))
        {
            try
            {
                bullet.tag = "PlayerBullet";
            }
            catch
            {
                // Si el tag no existe, usar el nombre
                bullet.name = "PlayerBullet";
            }
        }

        // Reproducir sonido de disparo
        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }
    }
}
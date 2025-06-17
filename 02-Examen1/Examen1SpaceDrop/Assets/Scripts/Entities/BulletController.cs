using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float speed = 10f;           // Velocidad de la bala
    public float lifetime = 3f;         // Tiempo de vida de la bala

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Verificar Rigidbody2D
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
        }

        // Mover la bala hacia arriba
        rb.linearVelocity = Vector2.up * speed;

        // Destruir la bala después del tiempo de vida
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Destruir la bala si sale de la pantalla por arriba
        if (transform.position.y > 8f)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Si choca con un cristal
        if (other.CompareTag("Crystal") || other.name.Contains("Crystal"))
        {
            // El cristal se encargará de destruirse y dar puntos
            // Solo destruimos la bala
            Destroy(gameObject);
        }
    }
}

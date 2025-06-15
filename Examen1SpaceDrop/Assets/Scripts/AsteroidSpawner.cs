using System.Collections;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    [Header("Configuración de Asteroides")]
    public GameObject[] asteroidPrefabs;    // Array de prefabs de asteroides diferentes
    public float minSpawnTime = 1f;         // Tiempo mínimo entre spawns
    public float maxSpawnTime = 3f;         // Tiempo máximo entre spawns
    public float spawnDistance = 8f;        // Distancia desde el centro para hacer spawn (REDUCIDO)
    public int maxAsteroids = 10;           // Número máximo de asteroides en pantalla
    public bool forceCloseSpawn = true;     // Forzar spawn cerca para debugging

    [Header("Configuración de Dificultad")]
    public float difficultyMultiplier = 0.95f;  // Factor de reducción del tiempo de spawn
    public float minSpawnTimeLimit = 0.5f;      // Límite mínimo del tiempo de spawn
    public float difficultyIncreaseTime = 30f;  // Cada cuánto aumenta la dificultad (segundos)

    private float nextSpawnTime;
    private int asteroidsInScene = 0;    void Start()
    {
        // Validar que haya al menos un prefab asignado
        if (asteroidPrefabs == null || asteroidPrefabs.Length == 0)
        {
            Debug.LogError("No hay prefabs de asteroides asignados al spawner. Asigna al menos uno en el inspector.");
            enabled = false; // Deshabilitar el script si no hay prefabs
            return;
        }

        // Verificar que los prefabs no sean null
        foreach (GameObject prefab in asteroidPrefabs)
        {
            if (prefab == null)
            {
                Debug.LogError("Uno de los prefabs de asteroides es null. Revisa los prefabs asignados al spawner.");
                // No deshabilitar el script, solo advertir
            }
        }
        // Asegurarse de que este objeto no se destruya fácilmente
        Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;        // Iniciar el spawner inmediatamente para el primer asteroide
        SpawnAsteroid();        // Iniciar el aumento de dificultad
        InvokeRepeating("IncreaseDifficulty", difficultyIncreaseTime, difficultyIncreaseTime);
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGameOver)
        {
            return; // No generar más asteroides si el juego ha terminado
        }

        // Verificar si es momento de generar un nuevo asteroide
        if (Time.time >= nextSpawnTime && asteroidsInScene < maxAsteroids)
        {
            SpawnAsteroid();            // Calcular el próximo tiempo de spawn
            float spawnDelay = Random.Range(minSpawnTime, maxSpawnTime);
            nextSpawnTime = Time.time + spawnDelay;
        }
    }
    void SpawnAsteroid()
    {
        if (asteroidPrefabs.Length == 0)
        {
            Debug.LogError("No hay prefabs de asteroides asignados al spawner. Asigna al menos uno en el inspector.");
            return;
        }

        // Elegir un prefab aleatorio del array
        int randomIndex = Random.Range(0, asteroidPrefabs.Length);
        GameObject asteroidPrefab = asteroidPrefabs[randomIndex];

        if (asteroidPrefab == null)
        {
            Debug.LogError("El prefab seleccionado es null. Revisa los prefabs asignados al spawner.");
            return;
        }

        // Determinar una posición aleatoria en el borde de la pantalla
        Vector2 spawnPosition = GetRandomSpawnPosition();

        // Instanciar el asteroide - NO como hijo para evitar problemas de visibilidad
        GameObject asteroid = Instantiate(asteroidPrefab, spawnPosition, Quaternion.identity);

        // Dar un nombre descriptivo al asteroide
        asteroid.name = "Asteroid_" + asteroidsInScene;

        if (asteroid == null)
        {
            Debug.LogError("No se pudo instanciar el asteroide.");
            return;
        }        asteroidsInScene++;

        // Configurar dirección hacia abajo con ligera variación aleatoria
        AsteroidController controller = asteroid.GetComponent<AsteroidController>();
        if (controller != null)
        {            // Calcular dirección hacia abajo con ligera variación horizontal
            Vector2 direction = new Vector2(
                Random.Range(-0.3f, 0.3f), // Pequeña variación horizontal
                -1f                         // Siempre hacia abajo
            ).normalized;

            controller.SetDirection(direction);
        }
        else
        {
            Debug.LogWarning("El asteroide no tiene componente AsteroidController. Asegúrate de que el prefab tiene este componente.");
        }        // Verificar que el asteroide tenga un sprite visible
        SpriteRenderer renderer = asteroid.GetComponent<SpriteRenderer>();
        if (renderer == null || renderer.sprite == null)
        {
            Debug.LogError($"El asteroide {asteroid.name} no tiene un SpriteRenderer con un sprite asignado.");
            // Solamente registramos el error, pero no creamos marcadores visuales para evitar fondos no deseados
        }        else
        {
            // El asteroide ya tiene un SpriteRenderer con un sprite asignado
        }

        // Destruir el asteroide después de un tiempo máximo por seguridad
        Destroy(asteroid, 15f);

        // Registrar la destrucción para el contador
        StartCoroutine(CountAsteroidDestruction(asteroid));
    }
    Vector2 GetRandomSpawnPosition()
    {
        // Ahora solo generamos asteroides desde la parte superior de la pantalla
        Vector2 position = Vector2.zero;

        // Si estamos forzando el spawn cercano para debugging, usar una distancia menor
        float actualDistance = forceCloseSpawn ? 5f : spawnDistance;        // Posición en la parte superior con posición X aleatoria
        position = new Vector2(Random.Range(-5f, 5f), actualDistance);

        return position;
    }

    IEnumerator CountAsteroidDestruction(GameObject asteroid)
    {        // Esperar hasta que el asteroide sea destruido
        yield return new WaitUntil(() => asteroid == null);

        // Decrementar el contador
        asteroidsInScene--;
    }

    void IncreaseDifficulty()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGameOver) return;

        // Reducir el tiempo de spawn para aumentar la dificultad
        minSpawnTime *= difficultyMultiplier;
        maxSpawnTime *= difficultyMultiplier;        // Asegurar que no baje del mínimo establecido
        minSpawnTime = Mathf.Max(minSpawnTime, minSpawnTimeLimit);
        maxSpawnTime = Mathf.Max(maxSpawnTime, minSpawnTime + 0.5f);
    }
}

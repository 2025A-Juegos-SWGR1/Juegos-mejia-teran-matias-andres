using System.Collections;
using UnityEngine;

public class CrystalSpawner : MonoBehaviour
{
    [Header("Configuración de Cristales")]
    public GameObject[] crystalPrefabs;     // Array de prefabs de cristales diferentes
    public float minSpawnTime = 3f;         // Tiempo mínimo entre spawns
    public float maxSpawnTime = 6f;         // Tiempo máximo entre spawns
    public float spawnDistance = 8f;        // Distancia desde el centro para hacer spawn
    public int maxCrystals = 5;             // Número máximo de cristales en pantalla
    public bool forceCloseSpawn = true;     // Forzar spawn cerca para debugging

    [Header("Configuración de Dificultad")]
    public float difficultyMultiplier = 0.98f;  // Factor de reducción del tiempo de spawn (más lento que asteroides)
    public float minSpawnTimeLimit = 1.5f;      // Límite mínimo del tiempo de spawn
    public float difficultyIncreaseTime = 45f;  // Cada cuánto aumenta la dificultad (segundos)

    private float nextSpawnTime;
    private int crystalsInScene = 0;

    void Start()
    {
        // Validar que haya al menos un prefab asignado
        if (crystalPrefabs == null || crystalPrefabs.Length == 0)
        {
            Debug.LogError("No hay prefabs de cristales asignados al spawner. Asigna al menos uno en el inspector.");
            enabled = false; // Deshabilitar el script si no hay prefabs
            return;
        }

        // Verificar que los prefabs no sean null
        foreach (GameObject prefab in crystalPrefabs)
        {
            if (prefab == null)
            {
                Debug.LogError("Uno de los prefabs de cristales es null. Revisa los prefabs asignados al spawner.");
            }
        }

        // Asegurarse de que este objeto no se destruya fácilmente
        Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;

        // Iniciar el spawner después de un pequeño retraso
        nextSpawnTime = Time.time + Random.Range(minSpawnTime, maxSpawnTime);

        // Iniciar el aumento de dificultad
        InvokeRepeating("IncreaseDifficulty", difficultyIncreaseTime, difficultyIncreaseTime);
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGameOver)
        {
            return; // No generar más cristales si el juego ha terminado
        }

        // Verificar si es momento de generar un nuevo cristal
        if (Time.time >= nextSpawnTime && crystalsInScene < maxCrystals)
        {
            SpawnCrystal();

            // Calcular el próximo tiempo de spawn
            float spawnDelay = Random.Range(minSpawnTime, maxSpawnTime);
            nextSpawnTime = Time.time + spawnDelay;
        }
    }

    void SpawnCrystal()
    {
        if (crystalPrefabs.Length == 0)
        {
            Debug.LogError("No hay prefabs de cristales asignados al spawner.");
            return;
        }

        // Elegir un prefab aleatorio del array
        int randomIndex = Random.Range(0, crystalPrefabs.Length);
        GameObject crystalPrefab = crystalPrefabs[randomIndex];

        if (crystalPrefab == null)
        {
            Debug.LogError("El prefab de cristal seleccionado es null.");
            return;
        }

        // Determinar una posición aleatoria en el borde superior de la pantalla
        Vector2 spawnPosition = GetRandomSpawnPosition();

        // Instanciar el cristal
        GameObject crystal = Instantiate(crystalPrefab, spawnPosition, Quaternion.identity);

        // Dar un nombre descriptivo al cristal
        crystal.name = "Crystal_" + crystalsInScene;

        if (crystal == null)
        {
            Debug.LogError("No se pudo instanciar el cristal.");
            return;
        }

        crystalsInScene++;

        // Configurar dirección hacia abajo con ligera variación aleatoria
        CrystalController controller = crystal.GetComponent<CrystalController>();
        if (controller != null)
        {
            // Calcular dirección hacia abajo con ligera variación horizontal
            Vector2 direction = new Vector2(
                Random.Range(-0.2f, 0.2f), // Pequeña variación horizontal (menos que asteroides)
                -1f                         // Siempre hacia abajo
            ).normalized;

            controller.SetDirection(direction);
        }
        else
        {
            Debug.LogWarning("El cristal no tiene componente CrystalController.");
        }

        // Verificar que el cristal tenga un sprite visible
        SpriteRenderer renderer = crystal.GetComponent<SpriteRenderer>();
        if (renderer == null || renderer.sprite == null)
        {
            Debug.LogError($"El cristal {crystal.name} no tiene un SpriteRenderer con un sprite asignado.");
        }

        // Destruir el cristal después de un tiempo máximo por seguridad
        Destroy(crystal, 20f);

        // Registrar la destrucción para el contador
        StartCoroutine(CountCrystalDestruction(crystal));
    }

    Vector2 GetRandomSpawnPosition()
    {
        Vector2 position = Vector2.zero;

        // Si estamos forzando el spawn cercano para debugging, usar una distancia menor
        float actualDistance = forceCloseSpawn ? 5f : spawnDistance;

        // Posición en la parte superior con posición X aleatoria
        position = new Vector2(Random.Range(-4f, 4f), actualDistance);

        return position;
    }

    IEnumerator CountCrystalDestruction(GameObject crystal)
    {
        // Esperar hasta que el cristal sea destruido
        yield return new WaitUntil(() => crystal == null);

        // Decrementar el contador
        crystalsInScene--;
    }

    void IncreaseDifficulty()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGameOver) return;

        // Reducir el tiempo de spawn para aumentar la dificultad
        minSpawnTime *= difficultyMultiplier;
        maxSpawnTime *= difficultyMultiplier;

        // Asegurar que no baje del mínimo establecido
        minSpawnTime = Mathf.Max(minSpawnTime, minSpawnTimeLimit);
        maxSpawnTime = Mathf.Max(maxSpawnTime, minSpawnTime + 1f);
    }
}

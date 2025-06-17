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
    public bool forceCloseSpawn = true;     // Forzar spawn cerca para debugging    [Header("Configuración de Tipos de Cristales")]
    [Range(0f, 100f)]
    public float yellowCrystalChance = 50f;  // 50% - Más común, 50 puntos
    [Range(0f, 100f)]
    public float blueCrystalChance = 30f;    // 30% - Común, 75 puntos
    [Range(0f, 100f)]
    public float redCrystalChance = 15f;     // 15% - Raro, 100 puntos
    [Range(0f, 100f)]
    public float greenCrystalChance = 5f;    // 5% - Muy raro, 150 puntos

    [Header("Configuración de Sprites de Cristales (Opcional)")]
    [Tooltip("Si asignas sprites aquí, se aplicarán a todos los cristales generados")]
    public Sprite yellowCrystalSprite;       // Sprite para cristales amarillos
    public Sprite blueCrystalSprite;         // Sprite para cristales azules
    public Sprite redCrystalSprite;          // Sprite para cristales rojos
    public Sprite greenCrystalSprite;        // Sprite para cristales verdes    [Header("Configuración de Dificultad")]
    public float difficultyMultiplier = 0.98f;  // Factor de reducción del tiempo de spawn (más lento que asteroides)
    public float minSpawnTimeLimit = 1.5f;      // Límite mínimo del tiempo de spawn
    public float difficultyIncreaseTime = 45f;  // Cada cuánto aumenta la dificultad (segundos)

    // Variables privadas
    private float nextSpawnTime;
    private int crystalsInScene = 0;
    private float[] normalizedChances; // Probabilidades normalizadas para los tipos de cristales

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

        // Normalizar las probabilidades de los tipos de cristales
        NormalizeProbabilities();

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
        // Verificar el estado del juego antes de generar cristales
        GameStateManager gameStateManager = GameStateManager.Instance;
        if (gameStateManager != null && !gameStateManager.IsInGame())
        {
            return; // No generar cristales si no estamos jugando
        }

        // Verificar si el GameManager indica que el juego ha terminado (comportamiento de respaldo)
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

        if (crystal == null)
        {
            Debug.LogError("No se pudo instanciar el cristal.");
            return;
        }        // Determinar el tipo de cristal basado en las probabilidades
        CrystalType crystalType = GetRandomCrystalType();

        // Configurar el tipo del cristal
        CrystalController controller = crystal.GetComponent<CrystalController>();
        if (controller != null)
        {
            // Asignar sprites globales si están configurados
            AssignSpritesToCrystal(controller);

            // Configurar el tipo del cristal (esto aplicará el sprite y color correspondiente)
            controller.SetCrystalType(crystalType);

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

        // Dar un nombre descriptivo al cristal
        crystal.name = $"Crystal_{crystalType}_{crystalsInScene}";

        crystalsInScene++;

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

    // Método para normalizar las probabilidades de los tipos de cristales
    private void NormalizeProbabilities()
    {
        float totalChance = yellowCrystalChance + blueCrystalChance + redCrystalChance + greenCrystalChance;

        if (totalChance <= 0)
        {
            // Si todas las probabilidades son 0, usar valores por defecto
            yellowCrystalChance = 50f;
            blueCrystalChance = 30f;
            redCrystalChance = 15f;
            greenCrystalChance = 5f;
            totalChance = 100f;
        }

        // Normalizar a 100%
        normalizedChances = new float[4];
        normalizedChances[0] = yellowCrystalChance / totalChance * 100f; // Yellow
        normalizedChances[1] = blueCrystalChance / totalChance * 100f;   // Blue
        normalizedChances[2] = redCrystalChance / totalChance * 100f;    // Red
        normalizedChances[3] = greenCrystalChance / totalChance * 100f;  // Green
    }

    // Método para obtener un tipo de cristal aleatorio basado en las probabilidades
    private CrystalType GetRandomCrystalType()
    {
        float randomValue = Random.Range(0f, 100f);
        float cumulativeChance = 0f;

        // Yellow Crystal
        cumulativeChance += normalizedChances[0];
        if (randomValue <= cumulativeChance)
            return CrystalType.Yellow;        // Blue Crystal
        cumulativeChance += normalizedChances[1];
        if (randomValue <= cumulativeChance)
            return CrystalType.Blue;

        // Red Crystal
        cumulativeChance += normalizedChances[2];
        if (randomValue <= cumulativeChance)
            return CrystalType.Red;

        // Green Crystal (por defecto si algo sale mal)
        return CrystalType.Green;
    }

    // Método para asignar sprites globales a un cristal
    private void AssignSpritesToCrystal(CrystalController controller)
    {
        // Solo asignar sprites si están configurados en el spawner
        if (yellowCrystalSprite != null)
            controller.yellowSprite = yellowCrystalSprite;

        if (blueCrystalSprite != null)
            controller.blueSprite = blueCrystalSprite;

        if (redCrystalSprite != null)
            controller.redSprite = redCrystalSprite;

        if (greenCrystalSprite != null)
            controller.greenSprite = greenCrystalSprite;
    }
}

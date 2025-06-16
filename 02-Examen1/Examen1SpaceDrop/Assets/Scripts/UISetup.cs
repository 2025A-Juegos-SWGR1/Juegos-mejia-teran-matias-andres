using UnityEngine;
using UnityEngine.UI;

// Script para crear una interfaz de usuario básica para el juego
public class UISetup : MonoBehaviour
{
    [Header("Referencias")]
    public GameManager gameManager;

    [Header("Prefabs (Opcional)")]
    public Text scoreTextPrefab;
    public GameObject gameOverPanelPrefab;

    // Indica si la UI ya ha sido configurada
    private bool uiConfigured = false;

    void Start()
    {
        Debug.Log("Iniciando UISetup para crear la interfaz de usuario...");
        SetupUI();
    }

    void OnEnable()
    {
        // Si este componente se activa después de Start, intentar configurar la UI
        if (!uiConfigured)
        {
            SetupUI();
        }
    }

    // Configura la UI básica para el juego
    public void SetupUI()
    {
        // Evitar configurar la UI más de una vez
        if (uiConfigured)
        {
            return;
        }

        // Asegurar que existe un GameManager
        if (gameManager == null)
        {
            Debug.Log("Buscando GameManager existente o creando uno nuevo...");
            gameManager = GameManager.GetInstance();

            if (gameManager == null)
            {
                Debug.LogError("No se pudo obtener o crear el GameManager. Verifica que exista el script GameManager.");
                return;
            }
            else
            {
                Debug.Log("GameManager obtenido correctamente: " + gameManager.name);
            }
        }        // Buscar o crear un Canvas
        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();

            // Configurar el canvas
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;            // Añadir un EventSystem si no existe
            if (FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }

            Debug.Log("Canvas y EventSystem creados automáticamente.");
        }

        // Crear texto de puntuación si no existe
        if (gameManager.scoreText == null)
        {
            GameObject scoreObj;

            if (scoreTextPrefab != null)
            {
                // Usar el prefab si está disponible
                scoreObj = Instantiate(scoreTextPrefab.gameObject, canvas.transform);
            }
            else
            {
                // Crear un texto básico si no hay prefab
                scoreObj = new GameObject("ScoreText");
                scoreObj.transform.SetParent(canvas.transform, false);
                Text scoreText = scoreObj.AddComponent<Text>();                // Configurar el texto
                scoreText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                scoreText.fontSize = 24;
                scoreText.color = Color.white;
                scoreText.alignment = TextAnchor.UpperLeft;
                scoreText.text = "Puntuación: 0";

                // Configurar el RectTransform
                RectTransform rt = scoreText.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector2(20, -20);
                rt.sizeDelta = new Vector2(200, 50);
                rt.anchorMin = new Vector2(0, 1);
                rt.anchorMax = new Vector2(0, 1);
                rt.pivot = new Vector2(0, 1);                // Asignar al GameManager y verificar que se haya asignado correctamente
                gameManager.scoreText = scoreText;

                if (gameManager.scoreText == null)
                {
                    Debug.LogError("No se pudo asignar el texto de puntuación al GameManager.");
                }
                else
                {
                    Debug.Log("Texto de puntuación creado y asignado al GameManager correctamente.");
                }
            }
        }

        // Crear texto de vidas si no existe
        if (gameManager.livesText == null)
        {
            Debug.Log("Creando texto de vidas...");
            GameObject livesObj;

            // Usar prefab si está disponible, sino crear uno genérico
            if (scoreTextPrefab != null)
            {
                // Usar el prefab del score text como base
                livesObj = Instantiate(scoreTextPrefab.gameObject, canvas.transform);
            }
            else
            {
                livesObj = new GameObject("LivesText");
                livesObj.transform.SetParent(canvas.transform, false);
                Text livesText = livesObj.AddComponent<Text>();

                // Configurar el texto
                livesText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                livesText.fontSize = 24;
                livesText.color = Color.white;
                livesText.alignment = TextAnchor.UpperRight;
                livesText.text = "Vidas: 4";

                // Configurar posición (esquina superior derecha)
                RectTransform rt = livesText.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(1, 1);
                rt.anchorMax = new Vector2(1, 1);
                rt.anchoredPosition = new Vector2(-20, -20);
                rt.sizeDelta = new Vector2(200, 30);
                rt.pivot = new Vector2(1, 1);

                // Asignar al GameManager
                gameManager.livesText = livesText;

                if (gameManager.livesText == null)
                {
                    Debug.LogError("No se pudo asignar el texto de vidas al GameManager.");
                }
                else
                {
                    Debug.Log("Texto de vidas creado y asignado al GameManager correctamente.");
                }
            }
        }

        // Crear panel de Game Over si no existe
        if (gameManager.gameOverUI == null)
        {
            if (gameOverPanelPrefab != null)
            {
                // Usar el prefab si está disponible
                GameObject gameOverObj = Instantiate(gameOverPanelPrefab, canvas.transform);
                gameManager.gameOverUI = gameOverObj;

                // Asegurarse de que esté inactivo al inicio
                gameOverObj.SetActive(false);
            }
            else
            {
                // Crear un panel básico si no hay prefab
                GameObject gameOverObj = new GameObject("GameOverPanel");
                gameOverObj.transform.SetParent(canvas.transform, false);

                // Añadir un panel de fondo
                Image panelImage = gameOverObj.AddComponent<Image>();
                panelImage.color = new Color(0, 0, 0, 0.8f); // Negro semi-transparente

                // Configurar el RectTransform del panel
                RectTransform panelRT = gameOverObj.GetComponent<RectTransform>();
                panelRT.anchorMin = new Vector2(0, 0);
                panelRT.anchorMax = new Vector2(1, 1);
                panelRT.offsetMin = Vector2.zero;
                panelRT.offsetMax = Vector2.zero;

                // Añadir texto de Game Over
                GameObject textObj = new GameObject("GameOverText");
                textObj.transform.SetParent(gameOverObj.transform, false);
                Text gameOverText = textObj.AddComponent<Text>();                // Configurar el texto
                gameOverText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                gameOverText.fontSize = 48;
                gameOverText.color = Color.red;
                gameOverText.alignment = TextAnchor.MiddleCenter;
                gameOverText.text = "¡GAME OVER!";

                // Configurar el RectTransform del texto
                RectTransform textRT = gameOverText.GetComponent<RectTransform>();
                textRT.anchorMin = new Vector2(0.5f, 0.5f);
                textRT.anchorMax = new Vector2(0.5f, 0.5f);
                textRT.anchoredPosition = new Vector2(0, 50);
                textRT.sizeDelta = new Vector2(400, 100);

                // Añadir texto de reinicio
                GameObject restartTextObj = new GameObject("RestartText");
                restartTextObj.transform.SetParent(gameOverObj.transform, false);
                Text restartText = restartTextObj.AddComponent<Text>();                // Configurar el texto
                restartText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                restartText.fontSize = 24;
                restartText.color = Color.white;
                restartText.alignment = TextAnchor.MiddleCenter;
                restartText.text = "Reiniciando en 3 segundos...";

                // Configurar el RectTransform del texto
                RectTransform restartTextRT = restartText.GetComponent<RectTransform>();
                restartTextRT.anchorMin = new Vector2(0.5f, 0.5f);
                restartTextRT.anchorMax = new Vector2(0.5f, 0.5f);
                restartTextRT.anchoredPosition = new Vector2(0, -50);
                restartTextRT.sizeDelta = new Vector2(400, 50);                // Asignar al GameManager y desactivar
                gameManager.gameOverUI = gameOverObj;
                gameOverObj.SetActive(false);

                if (gameManager.gameOverUI == null)
                {
                    Debug.LogError("No se pudo asignar el panel de Game Over al GameManager.");
                }
                else
                {
                    Debug.Log("Panel de Game Over creado y asignado al GameManager correctamente.");
                }
            }
        }

        // Marcar la UI como configurada para evitar configuraciones repetidas
        uiConfigured = true;
        Debug.Log("Configuración de UI completada con éxito.");
    }
}

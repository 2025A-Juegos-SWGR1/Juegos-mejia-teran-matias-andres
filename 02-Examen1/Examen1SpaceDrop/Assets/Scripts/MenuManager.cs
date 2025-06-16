using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Paneles de UI")]
    public GameObject mainMenuPanel;
    public GameObject gameplayUI;
    public GameObject gameOverPanel;
    public GameObject pausePanel;

    [Header("Elementos del Menú Principal")]
    public Text titleText;
    public Text mainMenuHighScoreText;
    public Button playButton;
    public Button exitButton;

    [Header("Elementos de Gameplay")]
    public Text scoreText; public Text livesText;
    public Text highScoreText;
    public Button pauseButton;

    [Header("Elementos de Game Over")]
    public Text gameOverTitleText;
    public Text finalScoreText;
    public Text gameOverHighScoreText;
    public Button restartButton;
    public Button mainMenuButton;

    [Header("Elementos de Pausa")]
    public Text pauseTitleText;
    public Button resumeButton;
    public Button pauseMainMenuButton;

    [Header("Configuración")]
    public string gameTitle = "Space Drop";

    [Header("Audio UI")]
    public AudioClip buttonClickSound;    // Sonido al hacer clic en botones
    public AudioClip buttonHoverSound;    // Sonido al pasar mouse sobre botones (opcional)

    // AudioSource para sonidos de UI
    private AudioSource uiAudioSource;

    // Referencias a sistemas
    private GameStateManager gameStateManager;
    private GameManager gameManager;

    // Singleton
    public static MenuManager Instance { get; private set; }

    void Awake()
    {
        // Configurar singleton
        if (Instance == null)
        {
            Instance = this;

            // Configurar AudioSource para sonidos de UI
            uiAudioSource = GetComponent<AudioSource>();
            if (uiAudioSource == null)
            {
                uiAudioSource = gameObject.AddComponent<AudioSource>();
            }
            // Configurar AudioSource para efectos (no música)
            uiAudioSource.loop = false;
            uiAudioSource.playOnAwake = false;

            Debug.Log("MenuManager: Instancia creada");
        }
        else
        {
            Debug.Log("MenuManager: Instancia duplicada destruida");
            Destroy(gameObject);
            return;
        }
    }
    void Start()
    {
        // Obtener referencias
        gameStateManager = GameStateManager.Instance;
        gameManager = GameManager.GetInstance();

        // Deshabilitar UISetup si existe para evitar conflictos
        DisableUISetupScript();

        // Configurar la UI
        SetupUI();

        // Manejar elementos de UI duplicados después de configurar
        StartCoroutine(DelayedDuplicateCheck());

        // Suscribirse a cambios de estado
        if (gameStateManager != null)
        {
            gameStateManager.OnStateChanged += OnGameStateChanged;
        }

        // Configurar botones
        SetupButtons();

        // Mostrar el menú principal inicialmente
        ShowMainMenu();
    }

    void OnDestroy()
    {
        // Desuscribirse de eventos
        if (gameStateManager != null)
        {
            gameStateManager.OnStateChanged -= OnGameStateChanged;
        }
    }

    private void SetupUI()
    {
        // Si no hay paneles asignados, crearlos automáticamente
        if (mainMenuPanel == null || gameplayUI == null || gameOverPanel == null)
        {
            CreateUIElements();
        }

        // Configurar textos
        if (titleText != null)
        {
            titleText.text = gameTitle;
        }

        // Asegurar que solo el menú principal esté visible al inicio
        SetAllPanelsInactive();
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
        }
    }
    private void CreateUIElements()
    {
        Debug.Log("MenuManager: Verificando y creando elementos de UI automáticamente...");

        // Verificar si ya hay elementos de UI existentes
        Text existingScoreText = FindAnyObjectByType<Text>();
        if (existingScoreText != null && existingScoreText.text.Contains("Puntuación"))
        {
            Debug.Log("MenuManager: Se detectó UI existente, integrando con el sistema nuevo...");
        }

        // Buscar o crear Canvas
        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            // Crear EventSystem si no existe
            if (FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }
        }
        else
        {
            Debug.Log("MenuManager: Canvas existente encontrado, usando el existente");
        }

        // Crear menú principal
        CreateMainMenuPanel(canvas);

        // Crear UI de gameplay
        CreateGameplayUI(canvas);

        // Crear panel de game over
        CreateGameOverPanel(canvas);

        // Crear panel de pausa
        CreatePausePanel(canvas);
    }
    private void CreateMainMenuPanel(Canvas canvas)
    {
        // Panel principal
        GameObject panel = new GameObject("MainMenuPanel");
        panel.transform.SetParent(canvas.transform, false);

        // Agregar RectTransform y Image
        RectTransform panelRT = panel.AddComponent<RectTransform>();
        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0.1f, 0.1f, 0.2f, 0.9f); // Azul oscuro semi-transparente

        panelRT.anchorMin = Vector2.zero;
        panelRT.anchorMax = Vector2.one;
        panelRT.offsetMin = Vector2.zero;
        panelRT.offsetMax = Vector2.zero;
        // Título del juego
        GameObject titleObj = new GameObject("TitleText");
        titleObj.transform.SetParent(panel.transform, false);

        // Agregar RectTransform y Text
        RectTransform titleRT = titleObj.AddComponent<RectTransform>();
        Text title = titleObj.AddComponent<Text>();
        title.text = gameTitle;
        title.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        title.fontSize = 60;
        title.color = Color.white;
        title.alignment = TextAnchor.MiddleCenter;
        title.fontStyle = FontStyle.Bold;

        titleRT.anchorMin = new Vector2(0.5f, 0.7f);
        titleRT.anchorMax = new Vector2(0.5f, 0.7f);
        titleRT.anchoredPosition = Vector2.zero;
        titleRT.sizeDelta = new Vector2(600, 100);

        // Texto de puntuación máxima en menú principal
        GameObject mainMenuHighScoreObj = new GameObject("MainMenuHighScoreText");
        mainMenuHighScoreObj.transform.SetParent(panel.transform, false);

        RectTransform mainMenuHSRT = mainMenuHighScoreObj.AddComponent<RectTransform>();
        Text mainMenuHighScore = mainMenuHighScoreObj.AddComponent<Text>();
        mainMenuHighScore.text = "Récord: 0";
        mainMenuHighScore.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        mainMenuHighScore.fontSize = 28;
        mainMenuHighScore.color = Color.yellow;
        mainMenuHighScore.alignment = TextAnchor.MiddleCenter;
        mainMenuHighScore.fontStyle = FontStyle.Bold;

        mainMenuHSRT.anchorMin = new Vector2(0.5f, 0.6f);
        mainMenuHSRT.anchorMax = new Vector2(0.5f, 0.6f);
        mainMenuHSRT.anchoredPosition = Vector2.zero;
        mainMenuHSRT.sizeDelta = new Vector2(400, 50);
        // Botón Jugar
        GameObject playButtonObj = CreateButton("PlayButton", "JUGAR", panel.transform, new Vector2(0.5f, 0.5f), new Vector2(200, 60));
        Button playBtn = playButtonObj.GetComponent<Button>();
        playBtn.onClick.AddListener(() => { PlayButtonSound(); StartGame(); });

        // Botón Salir
        GameObject exitButtonObj = CreateButton("ExitButton", "SALIR", panel.transform, new Vector2(0.5f, 0.35f), new Vector2(200, 60));
        Button exitBtn = exitButtonObj.GetComponent<Button>();
        exitBtn.onClick.AddListener(() => { PlayButtonSound(); QuitGame(); });

        // Asignar referencias
        mainMenuPanel = panel;
        titleText = title;
        mainMenuHighScoreText = mainMenuHighScore;
        playButton = playBtn;
        exitButton = exitBtn;
    }
    private void CreateGameplayUI(Canvas canvas)
    {
        // Panel de gameplay
        GameObject panel = new GameObject("GameplayUI");
        panel.transform.SetParent(canvas.transform, false);

        // Agregar RectTransform explícitamente
        RectTransform panelRT = panel.AddComponent<RectTransform>();
        panelRT.anchorMin = Vector2.zero;
        panelRT.anchorMax = Vector2.one;
        panelRT.offsetMin = Vector2.zero;
        panelRT.offsetMax = Vector2.zero;

        // Buscar elementos de UI existentes antes de crear nuevos
        Text existingScoreText = null;
        Text existingLivesText = null;

        // Buscar todos los textos existentes
        Text[] allTexts = FindObjectsByType<Text>(FindObjectsSortMode.None);
        foreach (Text text in allTexts)
        {
            if (text.text.Contains("Puntuación") && existingScoreText == null)
            {
                existingScoreText = text;
                Debug.Log("MenuManager: Encontrado texto de puntuación existente: " + text.name);
            }
            else if (text.text.Contains("Vidas") && existingLivesText == null)
            {
                existingLivesText = text;
                Debug.Log("MenuManager: Encontrado texto de vidas existente: " + text.name);
            }
        }

        // Configurar texto de puntuación
        Text score = null;
        if (existingScoreText != null)
        {
            // Usar el texto existente y moverlo al panel de gameplay
            score = existingScoreText;
            score.transform.SetParent(panel.transform, false);

            // Asegurar que tenga el posicionamiento correcto
            RectTransform scoreRT = score.GetComponent<RectTransform>();
            if (scoreRT != null)
            {
                scoreRT.anchorMin = new Vector2(0, 1);
                scoreRT.anchorMax = new Vector2(0, 1);
                scoreRT.anchoredPosition = new Vector2(20, -20);
                scoreRT.sizeDelta = new Vector2(200, 50);
                scoreRT.pivot = new Vector2(0, 1);
            }

            Debug.Log("MenuManager: Usando texto de puntuación existente");
        }
        else
        {
            // Crear nuevo texto de puntuación solo si no existe
            GameObject scoreObj = new GameObject("ScoreText");
            scoreObj.transform.SetParent(panel.transform, false);

            // Agregar RectTransform y Text
            RectTransform scoreRT = scoreObj.AddComponent<RectTransform>();
            score = scoreObj.AddComponent<Text>();
            score.text = "Puntuación: 0";
            score.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            score.fontSize = 24;
            score.color = Color.white;
            score.alignment = TextAnchor.UpperLeft;

            scoreRT.anchorMin = new Vector2(0, 1);
            scoreRT.anchorMax = new Vector2(0, 1);
            scoreRT.anchoredPosition = new Vector2(20, -20);
            scoreRT.sizeDelta = new Vector2(200, 50);
            scoreRT.pivot = new Vector2(0, 1);

            Debug.Log("MenuManager: Creado nuevo texto de puntuación");
        }

        // Configurar texto de vidas
        Text lives = null;
        if (existingLivesText != null)
        {
            // Usar el texto existente y moverlo al panel de gameplay
            lives = existingLivesText;
            lives.transform.SetParent(panel.transform, false);

            // Asegurar que tenga el posicionamiento correcto
            RectTransform livesRT = lives.GetComponent<RectTransform>();
            if (livesRT != null)
            {
                livesRT.anchorMin = new Vector2(1, 1);
                livesRT.anchorMax = new Vector2(1, 1);
                livesRT.anchoredPosition = new Vector2(-20, -20);
                livesRT.sizeDelta = new Vector2(200, 50);
                livesRT.pivot = new Vector2(1, 1);
            }

            Debug.Log("MenuManager: Usando texto de vidas existente");
        }
        else
        {
            // Crear nuevo texto de vidas solo si no existe
            GameObject livesObj = new GameObject("LivesText");
            livesObj.transform.SetParent(panel.transform, false);

            // Agregar RectTransform y Text
            RectTransform livesRT = livesObj.AddComponent<RectTransform>();
            lives = livesObj.AddComponent<Text>();
            lives.text = "Vidas: 4";
            lives.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            lives.fontSize = 24;
            lives.color = Color.white;
            lives.alignment = TextAnchor.UpperRight;

            livesRT.anchorMin = new Vector2(1, 1);
            livesRT.anchorMax = new Vector2(1, 1);
            livesRT.anchoredPosition = new Vector2(-20, -20);
            livesRT.sizeDelta = new Vector2(200, 50);
            livesRT.pivot = new Vector2(1, 1);

            Debug.Log("MenuManager: Creado nuevo texto de vidas");
        }

        // Buscar o crear texto de puntuación máxima
        Text highScore = null;
        Text[] allHighScoreTexts = FindObjectsByType<Text>(FindObjectsSortMode.None);
        foreach (Text text in allHighScoreTexts)
        {
            if (text.text.Contains("Récord") && highScore == null)
            {
                highScore = text;
                Debug.Log("MenuManager: Encontrado texto de puntuación máxima existente: " + text.name);
                break;
            }
        }

        if (highScore == null)
        {
            // Crear nuevo texto de puntuación máxima
            GameObject highScoreObj = new GameObject("HighScoreText");
            highScoreObj.transform.SetParent(panel.transform, false);

            // Agregar RectTransform y Text
            RectTransform highScoreRT = highScoreObj.AddComponent<RectTransform>();
            highScore = highScoreObj.AddComponent<Text>();
            highScore.text = "Récord: 0";
            highScore.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            highScore.fontSize = 20;
            highScore.color = Color.yellow;
            highScore.alignment = TextAnchor.UpperCenter;

            // Posicionar en la parte superior centro, debajo del botón de pausa
            highScoreRT.anchorMin = new Vector2(0.5f, 1);
            highScoreRT.anchorMax = new Vector2(0.5f, 1);
            highScoreRT.anchoredPosition = new Vector2(0, -80);
            highScoreRT.sizeDelta = new Vector2(200, 30);
            highScoreRT.pivot = new Vector2(0.5f, 1);

            Debug.Log("MenuManager: Creado nuevo texto de puntuación máxima");
        }
        else
        {
            // Usar el texto existente y moverlo al panel
            highScore.transform.SetParent(panel.transform, false);

            RectTransform highScoreRT = highScore.GetComponent<RectTransform>();
            if (highScoreRT != null)
            {
                highScoreRT.anchorMin = new Vector2(0.5f, 1);
                highScoreRT.anchorMax = new Vector2(0.5f, 1);
                highScoreRT.anchoredPosition = new Vector2(0, -80);
                highScoreRT.sizeDelta = new Vector2(200, 30);
                highScoreRT.pivot = new Vector2(0.5f, 1);
            }

            Debug.Log("MenuManager: Usando texto de puntuación máxima existente");
        }

        // Botón de pausa (esquina superior centro)
        GameObject pauseButtonObj = CreateButton("PauseButton", "| |", panel.transform, new Vector2(0.5f, 1f), new Vector2(60, 40));
        RectTransform pauseRT = pauseButtonObj.GetComponent<RectTransform>();
        pauseRT.anchoredPosition = new Vector2(0, -30);
        Button pauseBtn = pauseButtonObj.GetComponent<Button>();
        pauseBtn.onClick.AddListener(() => { PlayButtonSound(); PauseGame(); });// Asignar referencias
        gameplayUI = panel;
        scoreText = score;
        livesText = lives;
        highScoreText = highScore;
        pauseButton = pauseBtn;

        // Asignar al GameManager solo si no tiene referencias ya
        if (gameManager != null)
        {
            if (gameManager.scoreText == null)
            {
                gameManager.scoreText = score;
                Debug.Log("MenuManager: Texto de puntuación asignado al GameManager");
            }

            if (gameManager.livesText == null)
            {
                gameManager.livesText = lives;
                Debug.Log("MenuManager: Texto de vidas asignado al GameManager");
            }

            if (gameManager.highScoreText == null)
            {
                gameManager.highScoreText = highScore;
                Debug.Log("MenuManager: Texto de puntuación máxima asignado al GameManager");
            }
        }
    }
    private void CreateGameOverPanel(Canvas canvas)
    {
        // Panel de game over
        GameObject panel = new GameObject("GameOverPanel");
        panel.transform.SetParent(canvas.transform, false);

        // Agregar RectTransform y Image
        RectTransform panelRT = panel.AddComponent<RectTransform>();
        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0.8f, 0.1f, 0.1f, 0.9f); // Rojo semi-transparente

        panelRT.anchorMin = Vector2.zero;
        panelRT.anchorMax = Vector2.one;
        panelRT.offsetMin = Vector2.zero;
        panelRT.offsetMax = Vector2.zero;
        // Título "Game Over"
        GameObject titleObj = new GameObject("GameOverTitleText");
        titleObj.transform.SetParent(panel.transform, false);

        // Agregar RectTransform y Text
        RectTransform titleRT = titleObj.AddComponent<RectTransform>();
        Text title = titleObj.AddComponent<Text>();
        title.text = "¡GAME OVER!";
        title.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        title.fontSize = 48;
        title.color = Color.white;
        title.alignment = TextAnchor.MiddleCenter;
        title.fontStyle = FontStyle.Bold;

        titleRT.anchorMin = new Vector2(0.5f, 0.7f);
        titleRT.anchorMax = new Vector2(0.5f, 0.7f);
        titleRT.anchoredPosition = Vector2.zero;
        titleRT.sizeDelta = new Vector2(500, 80);
        // Texto de puntuación final
        GameObject finalScoreObj = new GameObject("FinalScoreText");
        finalScoreObj.transform.SetParent(panel.transform, false);

        // Agregar RectTransform y Text
        RectTransform finalScoreRT = finalScoreObj.AddComponent<RectTransform>();
        Text finalScore = finalScoreObj.AddComponent<Text>();
        finalScore.text = "Puntuación Final: 0";
        finalScore.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        finalScore.fontSize = 28;
        finalScore.color = Color.yellow;
        finalScore.alignment = TextAnchor.MiddleCenter;

        finalScoreRT.anchorMin = new Vector2(0.5f, 0.55f);
        finalScoreRT.anchorMax = new Vector2(0.5f, 0.55f);
        finalScoreRT.anchoredPosition = Vector2.zero;
        finalScoreRT.sizeDelta = new Vector2(400, 50);

        // Texto de puntuación máxima en Game Over
        GameObject gameOverHighScoreObj = new GameObject("GameOverHighScoreText");
        gameOverHighScoreObj.transform.SetParent(panel.transform, false);

        // Agregar RectTransform y Text
        RectTransform gameOverHighScoreRT = gameOverHighScoreObj.AddComponent<RectTransform>();
        Text gameOverHighScore = gameOverHighScoreObj.AddComponent<Text>();
        gameOverHighScore.text = "Récord: 0";
        gameOverHighScore.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        gameOverHighScore.fontSize = 22;
        gameOverHighScore.color = Color.cyan;
        gameOverHighScore.alignment = TextAnchor.MiddleCenter;
        gameOverHighScoreRT.anchorMin = new Vector2(0.5f, 0.45f);
        gameOverHighScoreRT.anchorMax = new Vector2(0.5f, 0.45f);
        gameOverHighScoreRT.anchoredPosition = Vector2.zero;
        gameOverHighScoreRT.sizeDelta = new Vector2(300, 40);
        // Botón Reiniciar (movido más abajo para dar espacio)
        GameObject restartButtonObj = CreateButton("RestartButton", "REINICIAR", panel.transform, new Vector2(0.5f, 0.35f), new Vector2(200, 60));
        Button restartBtn = restartButtonObj.GetComponent<Button>();
        restartBtn.onClick.AddListener(() => { PlayButtonSound(); RestartGame(); });

        // Botón Menú Principal (movido más abajo)
        GameObject menuButtonObj = CreateButton("MainMenuButton", "MENÚ PRINCIPAL", panel.transform, new Vector2(0.5f, 0.2f), new Vector2(250, 60));
        Button menuBtn = menuButtonObj.GetComponent<Button>();
        menuBtn.onClick.AddListener(() => { PlayButtonSound(); ReturnToMainMenu(); });// Asignar referencias
        gameOverPanel = panel;
        gameOverTitleText = title;
        finalScoreText = finalScore;
        gameOverHighScoreText = gameOverHighScore;
        restartButton = restartBtn;
        mainMenuButton = menuBtn;

        // Asignar al GameManager solo si no tiene panel ya
        if (gameManager != null)
        {
            if (gameManager.gameOverUI == null)
            {
                gameManager.gameOverUI = panel;
                Debug.Log("MenuManager: Panel de Game Over asignado al GameManager");
            }
            else
            {
                Debug.Log("MenuManager: GameManager ya tiene panel de Game Over, usando el nuevo para navegación");
            }
        }
    }
    private void CreatePausePanel(Canvas canvas)
    {
        // Panel de pausa
        GameObject panel = new GameObject("PausePanel");
        panel.transform.SetParent(canvas.transform, false);

        // Agregar RectTransform y Image
        RectTransform panelRT = panel.AddComponent<RectTransform>();
        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0.3f, 0.3f, 0.3f, 0.8f); // Gris semi-transparente

        panelRT.anchorMin = Vector2.zero;
        panelRT.anchorMax = Vector2.one;
        panelRT.offsetMin = Vector2.zero;
        panelRT.offsetMax = Vector2.zero;
        // Título "Pausa"
        GameObject titleObj = new GameObject("PauseTitleText");
        titleObj.transform.SetParent(panel.transform, false);

        // Agregar RectTransform y Text
        RectTransform titleRT = titleObj.AddComponent<RectTransform>();
        Text title = titleObj.AddComponent<Text>();
        title.text = "PAUSA";
        title.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        title.fontSize = 40;
        title.color = Color.white;
        title.alignment = TextAnchor.MiddleCenter;
        title.fontStyle = FontStyle.Bold;

        titleRT.anchorMin = new Vector2(0.5f, 0.6f);
        titleRT.anchorMax = new Vector2(0.5f, 0.6f);
        titleRT.anchoredPosition = Vector2.zero;
        titleRT.sizeDelta = new Vector2(300, 60);
        // Botón Reanudar
        GameObject resumeButtonObj = CreateButton("ResumeButton", "REANUDAR", panel.transform, new Vector2(0.5f, 0.45f), new Vector2(200, 60));
        Button resumeBtn = resumeButtonObj.GetComponent<Button>();
        resumeBtn.onClick.AddListener(() => { PlayButtonSound(); ResumeGame(); });

        // Botón Menú Principal
        GameObject menuButtonObj = CreateButton("PauseMainMenuButton", "MENÚ PRINCIPAL", panel.transform, new Vector2(0.5f, 0.3f), new Vector2(250, 60));
        Button menuBtn = menuButtonObj.GetComponent<Button>();
        menuBtn.onClick.AddListener(() => { PlayButtonSound(); ReturnToMainMenu(); });

        // Asignar referencias
        pausePanel = panel;
        pauseTitleText = title;
        resumeButton = resumeBtn;
        pauseMainMenuButton = menuBtn;
    }
    private GameObject CreateButton(string name, string text, Transform parent, Vector2 anchorPosition, Vector2 size)
    {
        // Crear el GameObject del botón
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent, false);

        // Agregar componentes en el orden correcto: RectTransform, Image, Button
        RectTransform buttonRT = buttonObj.AddComponent<RectTransform>();
        Image buttonImage = buttonObj.AddComponent<Image>();
        Button button = buttonObj.AddComponent<Button>();

        // Configurar imagen del botón
        buttonImage.color = new Color(0.2f, 0.6f, 1f, 0.8f); // Azul semi-transparente

        // Configurar el RectTransform del botón
        buttonRT.anchorMin = anchorPosition;
        buttonRT.anchorMax = anchorPosition;
        buttonRT.anchoredPosition = Vector2.zero;
        buttonRT.sizeDelta = size;
        // Crear el texto del botón
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);

        // Agregar RectTransform y Text
        RectTransform textRT = textObj.AddComponent<RectTransform>();
        Text buttonText = textObj.AddComponent<Text>();
        buttonText.text = text;
        buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        buttonText.fontSize = 18;
        buttonText.color = Color.white;
        buttonText.alignment = TextAnchor.MiddleCenter;
        buttonText.fontStyle = FontStyle.Bold;

        // Configurar el RectTransform del texto
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.offsetMin = Vector2.zero;
        textRT.offsetMax = Vector2.zero;

        // Configurar el target del botón
        button.targetGraphic = buttonImage;

        return buttonObj;
    }

    private void SetupButtons()
    {
        // Los botones ya están configurados en los métodos Create...Panel()
        Debug.Log("MenuManager: Botones configurados");
    }

    private void OnGameStateChanged(GameState newState)
    {
        switch (newState)
        {
            case GameState.MainMenu:
                ShowMainMenu();
                break;

            case GameState.Playing:
                ShowGameplayUI();
                break;

            case GameState.Paused:
                ShowPauseMenu();
                break;

            case GameState.GameOver:
                ShowGameOverMenu();
                break;
        }
    }
    public void ShowMainMenu()
    {
        // Actualizar puntuación máxima en menú principal
        if (mainMenuHighScoreText != null && gameManager != null)
        {
            mainMenuHighScoreText.text = $"Récord: {gameManager.GetHighScore()}";
        }

        SetAllPanelsInactive();
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
        }
        Debug.Log("MenuManager: Menú principal mostrado");
    }

    public void ShowGameplayUI()
    {
        SetAllPanelsInactive();
        if (gameplayUI != null)
        {
            gameplayUI.SetActive(true);
        }
        Debug.Log("MenuManager: UI de gameplay mostrada");
    }
    public void ShowGameOverMenu()
    {
        // Actualizar puntuación final
        if (finalScoreText != null && gameManager != null)
        {
            finalScoreText.text = $"Puntuación Final: {gameManager.score}";
        }

        // Actualizar puntuación máxima
        if (gameOverHighScoreText != null && gameManager != null)
        {
            gameOverHighScoreText.text = $"Récord: {gameManager.GetHighScore()}";
        }

        SetAllPanelsInactive();
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        Debug.Log("MenuManager: Menú de Game Over mostrado");
    }

    public void ShowPauseMenu()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }
        Debug.Log("MenuManager: Menú de pausa mostrado");
    }

    private void SetAllPanelsInactive()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (gameplayUI != null) gameplayUI.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    // Métodos de botones
    public void StartGame()
    {
        Debug.Log("MenuManager: Iniciando juego...");

        // Resetear el juego
        if (gameManager != null)
        {
            gameManager.ResetGame();
        }

        // Cambiar estado
        if (gameStateManager != null)
        {
            gameStateManager.StartGame();
        }
    }
    public void PauseGame()
    {
        Debug.Log("MenuManager: Pausando juego...");

        // Pausar música
        if (gameManager != null)
        {
            gameManager.PauseMusic();
        }

        if (gameStateManager != null)
        {
            gameStateManager.PauseGame();
        }
    }

    public void ResumeGame()
    {
        Debug.Log("MenuManager: Reanudando juego...");

        // Reanudar música
        if (gameManager != null)
        {
            gameManager.ResumeMusic();
        }

        if (gameStateManager != null)
        {
            gameStateManager.ResumeGame();
        }
    }
    public void RestartGame()
    {
        Debug.Log("MenuManager: Reiniciando juego...");

        // Usar el método de reinicio completo del GameManager que recarga la escena
        if (gameManager != null)
        {
            gameManager.RestartGame();
        }
        else
        {
            // Fallback: recargar escena manualmente
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }

    public void ReturnToMainMenu()
    {
        Debug.Log("MenuManager: Regresando al menú principal...");

        // Resetear el juego
        if (gameManager != null)
        {
            gameManager.ResetGame();
        }

        // Cambiar estado
        if (gameStateManager != null)
        {
            gameStateManager.ReturnToMainMenu();
        }
    }

    public void QuitGame()
    {
        Debug.Log("MenuManager: Saliendo del juego...");

        if (gameStateManager != null)
        {
            gameStateManager.QuitGame();
        }
    }

    // Métodos para actualizar UI durante el juego
    public void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Puntuación: {score}";
        }
    }

    public void UpdateLives(int lives)
    {
        if (livesText != null)
        {
            livesText.text = $"Vidas: {lives}";
        }
    }

    public void UpdateHighScore(int highScore)
    {
        if (highScoreText != null)
        {
            highScoreText.text = $"Récord: {highScore}";
        }
    }

    // Método para reproducir sonido de botón
    private void PlayButtonSound()
    {
        if (uiAudioSource != null && buttonClickSound != null)
        {
            uiAudioSource.PlayOneShot(buttonClickSound);
        }
    }

    // Método para manejar la tecla ESC o pause
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameStateManager != null)
            {
                if (gameStateManager.IsInGame())
                {
                    PauseGame();
                }
                else if (gameStateManager.IsPaused())
                {
                    ResumeGame();
                }
            }
        }
    }

    // Método para detectar y manejar elementos de UI duplicados
    private void HandleDuplicateUIElements()
    {
        Debug.Log("MenuManager: Verificando elementos de UI duplicados...");

        // Buscar todos los textos de puntuación
        Text[] allTexts = FindObjectsByType<Text>(FindObjectsSortMode.None);
        int scoreTextCount = 0;
        int livesTextCount = 0;

        foreach (Text text in allTexts)
        {
            if (text.text.Contains("Puntuación"))
            {
                scoreTextCount++;
                // Si hay más de uno, ocultar los adicionales pero no el que está asignado al GameManager
                if (scoreTextCount > 1 && gameManager != null && text != gameManager.scoreText)
                {
                    text.gameObject.SetActive(false);
                    Debug.Log("MenuManager: Texto de puntuación duplicado ocultado: " + text.name);
                }
            }
            else if (text.text.Contains("Vidas"))
            {
                livesTextCount++;
                // Si hay más de uno, ocultar los adicionales pero no el que está asignado al GameManager
                if (livesTextCount > 1 && gameManager != null && text != gameManager.livesText)
                {
                    text.gameObject.SetActive(false);
                    Debug.Log("MenuManager: Texto de vidas duplicado ocultado: " + text.name);
                }
            }
        }

        if (scoreTextCount > 1)
        {
            Debug.LogWarning($"MenuManager: Se detectaron {scoreTextCount} textos de puntuación, algunos fueron ocultados");
        }

        if (livesTextCount > 1)
        {
            Debug.LogWarning($"MenuManager: Se detectaron {livesTextCount} textos de vidas, algunos fueron ocultados");
        }
    }

    private void DisableUISetupScript()
    {
        // Buscar y deshabilitar UISetup para evitar conflictos
        UISetup uiSetup = FindAnyObjectByType<UISetup>();
        if (uiSetup != null)
        {
            Debug.Log("MenuManager: Script UISetup encontrado, deshabilitándolo para evitar duplicados");
            uiSetup.enabled = false;
        }
    }

    private System.Collections.IEnumerator DelayedDuplicateCheck()
    {
        // Esperar un frame para que todos los scripts se inicialicen
        yield return new WaitForEndOfFrame();

        // Verificar y manejar duplicados después de que todo esté configurado
        HandleDuplicateUIElements();
    }
}
